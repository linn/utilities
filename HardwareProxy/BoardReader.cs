using HardwareProxy.Exceptions;
using Linn.Common.Configuration;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HardwareProxy
{
    public class BoardReader
    {
        private readonly string pathReadPcb;

        private readonly string serialPort;

        public BoardReader(string serialPort)
        {
            this.pathReadPcb = ConfigurationManager.Configuration["ReadPcb"];
            this.serialPort = serialPort;
        }

        public HardwareDescriptor Read(int clickCount = 1)
        {
            this.Read(clickCount, out var boardId);

            return new HardwareDescriptor
            {
                BoardId = boardId,
                BoardType = null,
                ProductId = 0,
                MacAddress = null
            };
        }

        private void Read(int clickCount, out string boardId)
        {
            // """
            // Board ID: 4300000279083514   Board Type: PCAS667P1R1
            // Product ID: 4321004    MAC address: 40:00:00:02:00:04
            // """
            //
            // """
            // Board ID: 71000002AE53FF14   Board Type: PCAS679P1R1
            // Product ID: (none)    MAC address: (none)
            // """
            const int NoPcbAttached = 6; // see ReadPcb.cpp
            const int CannotOpenPort = 3; 

            Regex boardIdRegex = new Regex(@"Board ID: (?<boardId>[0-9A-F]+)");
            Regex boardTypeRegex = new Regex(@"Board Type: (?<boardType>PCAS[0-9]+[LP][0-9]+R[0-9]+)");
            Regex productIdRegex = new Regex(@"Product ID: (?<prodId>([0-9]+|\(none\)))");
            Regex macAddressRegex = new Regex(@"MAC [Aa]ddress: (?<macAddr>([0-9A-Fa-f:]+|\(none\)))");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = this.pathReadPcb,
                Arguments = $"{this.serialPort} PRODUCTID:true ERRORSHOW:false",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var process = Process.Start(processStartInfo);
            var sRd = process.StandardOutput;
            var stringOutput = sRd.ReadToEnd();
            process.WaitForExit();

            // gracefully handle "no board attached"
            if (process.ExitCode == NoPcbAttached)
            {
                throw new NoBoardAttachedException("No board attached reported by ReadPCB");
            }

            if (process.ExitCode == CannotOpenPort)
            {
                throw new PortFailureException($"Cannot open port {this.serialPort}");
            }

            var startPosition = 0;
            var searchPosition = 0;
            for (var i = 1; i <= clickCount; i++)
            {
                searchPosition = stringOutput.IndexOf("Board ID:", searchPosition, StringComparison.Ordinal);
                if (searchPosition >= 0)
                {
                    startPosition = searchPosition;
                    searchPosition = startPosition + 1;
                }
                else
                {
                    startPosition = 0;
                    searchPosition = 1;
                }
            }

            // mandatory
            var match = boardIdRegex.Match(stringOutput, startPosition);

            // NOTE -- currently only using 48-bit board IDs, i.e. always
            // ... discarding leading 8-bit CRC and trailing 8-bit family code (0x14)
            if (match.Success)
            {
                boardId = match.Groups["boardId"].ToString().Substring(2, 12).ToUpper();
            }
            else
            {
                throw new MatchFailureException("Match failure");
            }
        }
    }
}

