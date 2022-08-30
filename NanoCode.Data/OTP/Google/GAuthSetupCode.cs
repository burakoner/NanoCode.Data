﻿namespace NanoCode.Data.OTP.Google
{
    public class GAuthSetupCode
    {
        public string Account { get; internal set; }
        public string AccountSecretKey { get; internal set; }
        public string ManualEntryKey { get; internal set; }
        public string QRCodeImageUrl { get; internal set; }
    }
}
