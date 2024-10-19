using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace HelloWorld.Utils
{
    public static class Validation
    {
        private static readonly Regex HostnameRegex = new(@"^[a-zA-Z0-9_-]{6,36}$");
        private static readonly HashSet<char> HostnameCharacters = new(64);

        private static readonly Regex IPv4AddressRegex = new(@"^(([1-9])[\d]{0,2}\.)(((([1-9])[\d]{0,2})|0)\.){2}(([1-9])[\d]{0,2})$");
        private static readonly Regex IPv6AddressRegex = new(@"^([\da-fA-F]{1,4}:){6}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^::([\da-fA-F]{1,4}:){0,4}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:):([\da-fA-F]{1,4}:){0,3}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){2}:([\da-fA-F]{1,4}:){0,2}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){3}:([\da-fA-F]{1,4}:){0,1}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){4}:((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$");
        private static readonly Regex InvalidIPv6AddressRegex = new(@"(^(0000:){7}(0000)$)|(^([Ff]{4}:){7}([Ff]{4})$)");
        private static readonly HashSet<char> IPAddressCharacters = new(24);
        static Validation()
        {
            foreach (var c in "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-")
            {
                HostnameCharacters.Add(c);
            }
            foreach (var c in "0123456789ABCDEFabcdef:.")
            {
                IPAddressCharacters.Add(c);
            }
        }

        public static bool IsPartOfHostname(string part)
        {
            return part.All(c => HostnameCharacters.Contains(c));
        }

        public static bool IsHostname(string hostname)
        {
            return HostnameRegex.IsMatch(hostname);
        }

        public static bool IsPartOfIPAddress(string part)
        {
            return part.All(c => IPAddressCharacters.Contains(c));
        }

        public static bool IsIPAddress(string ip)
        {
            var isValidIPv4 = false;
            if (IPv4AddressRegex.IsMatch(ip))
            {
                var parts = ip.Split('.');
                if (parts.Length == 4)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        var part = parts[i];
                        try
                        {
                            var num = int.Parse(part);
                            switch (i)
                            {
                                case 0:
                                case 3:
                                    isValidIPv4 = num > 0 && num < 255;
                                    break;
                                case 1:
                                case 2:
                                    isValidIPv4 = num >= 0 && num < 255;
                                    break;
                            }
                        }
                        catch (FormatException)
                        {
                            isValidIPv4 = false;
                        }
                    }
                }
            }

            if (isValidIPv4)
            {
                return true;
            }

            return IPv6AddressRegex.IsMatch(ip) && !InvalidIPv6AddressRegex.IsMatch(ip);
        }
    }
}
