using RestaurantPOS.Helpers.RequestDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RestaurantPOS.Helpers.UtilityHelper
{
    public static class GeneralHelper
    {
        private static Dictionary<string, string> contentTypes;
        public static int GenerateRandomNumbers(int length)
        {
            //int NoDigits = 4;
            Random rnd = new Random();
            int result = Convert.ToInt32(rnd.Next((int)Math.Pow(10, (length - 1)), (int)Math.Pow(10, length) - 1).ToString());
            return result;
        }
        public static string Rand()
        {
            int size = 8; bool lowerCase = false;
            var builder = new StringBuilder(size);
            Random _random = new Random();

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }


        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException )
            {

                return false;
            }
            catch (ArgumentException )
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static bool IsMobileNumber(string number)
        {
            if (number.Length < 15)
                return Regex.Match(number, @"\(?\d{3}\)?[. -]? *\d{3}[. -]? *[. -]?\d{4}").Success;
            else
                return false;
        }
        public static string GetDefaultProfilePicture()
        {
            return "uploads/defaultprofilepicture.jpg";
        }
        /// <summary>
        /// Given a file name, determines the MIME type
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>String containing the MIME type</returns>
        public static string GetContentType(string fileName)
        {
            if (contentTypes == null || !(contentTypes.Count > 0))
            {
                InitializeMimeTypes();
            }
            string extension;

            var fi = new FileInfo(fileName);
            extension = fi.Extension.Replace(".", "");


            contentTypes.TryGetValue(extension.ToLower(), out string contentType);


            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        private static void InitializeMimeTypes()
        {
            contentTypes = new Dictionary<string, string>
                               {
                                   {"3dm", "x-world/x-3dmf"},
                                   {"3dmf", "x-world/x-3dmf"},
                                   {"a", "application/octet-stream"},
                                   {"aab", "application/x-authorware-bin"},
                                   {"aam", "application/x-authorware-map"},
                                   {"aas", "application/x-authorware-seg"},
                                   {"abc", "text/vnd.abc"},
                                   {"acgi", "text/html"},
                                   {"afl", "video/animaflex"},
                                   {"ai", "application/postscript"},
                                   {"aif", "audio/aiff"},
                                   {"aifc", "audio/aiff"},
                                   {"aiff", "audio/aiff"},
                                   {"aim", "application/x-aim"},
                                   {"aip", "text/x-audiosoft-intra"},
                                   {"ani", "application/x-navi-animation"},
                                   {"aos", "application/x-nokia-9000-communicator-add-on-software"},
                                   {"aps", "application/mime"},
                                   {"arc", "application/octet-stream"},
                                   {"arj", "application/arj"},
                                   {"art", "image/x-jg"},
                                   {"asf", "video/x-ms-asf"},
                                   {"asm", "text/x-asm"},
                                   {"asp", "text/asp"},
                                   {"asx", "application/x-mplayer2"},
                                   {"au", "audio/basic"},
                                   {"avi", "video/avi"},
                                   {"avs", "video/avs-video"},
                                   {"bcpio", "application/x-bcpio"},
                                   {"bin", "application/octet-stream"},
                                   {"bm", "image/bmp"},
                                   {"bmp", "image/bmp"},
                                   {"boo", "application/book"},
                                   {"book", "application/book"},
                                   {"boz", "application/x-bzip2"},
                                   {"bsh", "application/x-bsh"},
                                   {"bz", "application/x-bzip"},
                                   {"bz2", "application/x-bzip2"},
                                   {"c", "text/plain"},
                                   {"c++", "text/plain"},
                                   {"cat", "application/vnd.ms-pki.seccat"},
                                   {"cc", "text/plain"},
                                   {"ccad", "application/clariscad"},
                                   {"cco", "application/x-cocoa"},
                                   {"cdf", "application/cdf"},
                                   {"cer", "application/pkix-cert"},
                                   {"cha", "application/x-chat"},
                                   {"chat", "application/x-chat"},
                                   {"class", "application/java"},
                                   {"com", "application/octet-stream"},
                                   {"conf", "text/plain"},
                                   {"cpio", "application/x-cpio"},
                                   {"cpp", "text/x-c"},
                                   {"cpt", "application/x-cpt"},
                                   {"crl", "application/pkcs-crl"},
                                   {"css", "text/css"},
                                   {"def", "text/plain"},
                                   {"der", "application/x-x509-ca-cert"},
                                   {"dif", "video/x-dv"},
                                   {"dir", "application/x-director"},
                                   {"dl", "video/dl"},
                                   {"doc", "application/msword"},
                                   {"dot", "application/msword"},
                                   {"dp", "application/commonground"},
                                   {"drw", "application/drafting"},
                                   {"dump", "application/octet-stream"},
                                   {"dv", "video/x-dv"},
                                   {"dvi", "application/x-dvi"},
                                   {"dwf", "drawing/x-dwf (old)"},
                                   {"dwg", "application/acad"},
                                   {"dxf", "application/dxf"},
                                   {"eps", "application/postscript"},
                                   {"es", "application/x-esrehber"},
                                   {"etx", "text/x-setext"},
                                   {"evy", "application/envoy"},
                                   {"exe", "application/octet-stream"},
                                   {"f", "text/plain"},
                                   {"f90", "text/x-fortran"},
                                   {"fdf", "application/vnd.fdf"},
                                   {"fif", "image/fif"},
                                   {"fli", "video/fli"},
                                   {"for", "text/x-fortran"},
                                   {"fpx", "image/vnd.fpx"},
                                   {"g", "text/plain"},
                                   {"g3", "image/g3fax"},
                                   {"gif", "image/gif"},
                                   {"gl", "video/gl"},
                                   {"gsd", "audio/x-gsm"},
                                   {"gtar", "application/x-gtar"},
                                   {"gz", "application/x-compressed"},
                                   {"h", "text/plain"},
                                   {"help", "application/x-helpfile"},
                                   {"hgl", "application/vnd.hp-hpgl"},
                                   {"hh", "text/plain"},
                                   {"hlp", "application/x-winhelp"},
                                   {"htc", "text/x-component"},
                                   {"htm", "text/html"},
                                   {"html", "text/html"},
                                   {"htmls", "text/html"},
                                   {"htt", "text/webviewhtml"},
                                   {"htx", "text/html"},
                                   {"ice", "x-conference/x-cooltalk"},
                                   {"ico", "image/x-icon"},
                                   {"idc", "text/plain"},
                                   {"ief", "image/ief"},
                                   {"iefs", "image/ief"},
                                   {"iges", "application/iges"},
                                   {"igs", "application/iges"},
                                   {"ima", "application/x-ima"},
                                   {"imap", "application/x-httpd-imap"},
                                   {"inf", "application/inf"},
                                   {"ins", "application/x-internett-signup"},
                                   {"ip", "application/x-ip2"},
                                   {"isu", "video/x-isvideo"},
                                   {"it", "audio/it"},
                                   {"iv", "application/x-inventor"},
                                   {"ivr", "i-world/i-vrml"},
                                   {"ivy", "application/x-livescreen"},
                                   {"jam", "audio/x-jam"},
                                   {"jav", "text/plain"},
                                   {"java", "text/plain"},
                                   {"jcm", "application/x-java-commerce"},
                                   {"jfif", "image/jpeg"},
                                   {"jfif-tbnl", "image/jpeg"},
                                   {"jpe", "image/jpeg"},
                                   {"jpeg", "image/jpeg"},
                                   {"jpg", "image/jpeg"},
                                   {"jps", "image/x-jps"},
                                   {"js", "application/x-javascript"},
                                   {"jut", "image/jutvision"},
                                   {"kar", "audio/midi"},
                                   {"ksh", "application/x-ksh"},
                                   {"la", "audio/nspaudio"},
                                   {"lam", "audio/x-liveaudio"},
                                   {"latex", "application/x-latex"},
                                   {"lha", "application/lha"},
                                   {"lhx", "application/octet-stream"},
                                   {"list", "text/plain"},
                                   {"lma", "audio/nspaudio"},
                                   {"log", "text/plain"},
                                   {"lsp", "application/x-lisp"},
                                   {"lst", "text/plain"},
                                   {"lsx", "text/x-la-asf"},
                                   {"ltx", "application/x-latex"},
                                   {"lzh", "application/octet-stream"},
                                   {"lzx", "application/lzx"},
                                   {"m", "text/plain"},
                                   {"m1v", "video/mpeg"},
                                   {"m2a", "audio/mpeg"},
                                   {"m2v", "video/mpeg"},
                                   {"m3u", "audio/x-mpequrl"},
                                   {"man", "application/x-troff-man"},
                                   {"map", "application/x-navimap"},
                                   {"mar", "text/plain"},
                                   {"mbd", "application/mbedlet"},
                                   {"mc$", "application/x-magic-cap-package-1.0"},
                                   {"mcd", "application/mcad"},
                                   {"mcf", "image/vasa"},
                                   {"mcp", "application/netmc"},
                                   {"me", "application/x-troff-me"},
                                   {"mht", "message/rfc822"},
                                   {"mhtml", "message/rfc822"},
                                   {"mid", "audio/midi"},
                                   {"midi", "audio/midi"},
                                   {"mif", "application/x-frame"},
                                   {"mime", "message/rfc822"},
                                   {"mjf", "audio/x-vnd.audioexplosion.mjuicemediafile"},
                                   {"mjpg", "video/x-motion-jpeg"},
                                   {"mm", "application/base64"},
                                   {"mme", "application/base64"},
                                   {"mod", "audio/mod"},
                                   {"moov", "video/quicktime"},
                                   {"mov", "video/quicktime"},
                                   {"movie", "video/x-sgi-movie"},
                                   {"mp2", "audio/mpeg"},
                                   {"mp3", "audio/mpeg3"},
                                   {"mpa", "audio/mpeg"},
                                   {"mpc", "application/x-project"},
                                   {"mpe", "video/mpeg"},
                                   {"mpeg", "video/mpeg"},
                                   {"mpg", "video/mpeg"},
                                   {"mpga", "audio/mpeg"},
                                   {"mpp", "application/vnd.ms-project"},
                                   {"mpt", "application/x-project"},
                                   {"mpv", "application/x-project"},
                                   {"mpx", "application/x-project"},
                                   {"mrc", "application/marc"},
                                   {"ms", "application/x-troff-ms"},
                                   {"mv", "video/x-sgi-movie"},
                                   {"my", "audio/make"},
                                   {"mzz", "application/x-vnd.audioexplosion.mzz"},
                                   {"nap", "image/naplps"},
                                   {"naplps", "image/naplps"},
                                   {"nc", "application/x-netcdf"},
                                   {"ncm", "application/vnd.nokia.configuration-message"},
                                   {"nif", "image/x-niff"},
                                   {"niff", "image/x-niff"},
                                   {"nix", "application/x-mix-transfer"},
                                   {"nsc", "application/x-conference"},
                                   {"nvd", "application/x-navidoc"},
                                   {"o", "application/octet-stream"},
                                   {"oda", "application/oda"},
                                   {"omc", "application/x-omc"},
                                   {"omcd", "application/x-omcdatamaker"},
                                   {"omcr", "application/x-omcregerator"},
                                   {"p", "text/x-pascal"},
                                   {"p10", "application/pkcs10"},
                                   {"p12", "application/pkcs-12"},
                                   {"p7a", "application/x-pkcs7-signature"},
                                   {"p7c", "application/pkcs7-mime"},
                                   {"pas", "text/pascal"},
                                   {"pbm", "image/x-portable-bitmap"},
                                   {"pcl", "application/vnd.hp-pcl"},
                                   {"pct", "image/x-pict"},
                                   {"pcx", "image/x-pcx"},
                                   {"pdf", "application/pdf"},
                                   {"epub", "application/epub+zip"},
                                   {"pfunk", "audio/make"},
                                   {"pgm", "image/x-portable-graymap"},
                                   {"pic", "image/pict"},
                                   {"pict", "image/pict"},
                                   {"pkg", "application/x-newton-compatible-pkg"},
                                   {"pko", "application/vnd.ms-pki.pko"},
                                   {"pl", "text/plain"},
                                   {"plx", "application/x-pixclscript"},
                                   {"pm", "image/x-xpixmap"},
                                   {"png", "image/png"},
                                   {"pnm", "application/x-portable-anymap"},
                                   {"pot", "application/mspowerpoint"},
                                   {"pov", "model/x-pov"},
                                   {"ppa", "application/vnd.ms-powerpoint"},
                                   {"ppm", "image/x-portable-pixmap"},
                                   {"pps", "application/mspowerpoint"},
                                   {"ppt", "application/mspowerpoint"},
                                   {"ppz", "application/mspowerpoint"},
                                   {"pre", "application/x-freelance"},
                                   {"prt", "application/pro_eng"},
                                   {"ps", "application/postscript"},
                                   {"psd", "application/octet-stream"},
                                   {"pvu", "paleovu/x-pv"},
                                   {"pwz", "application/vnd.ms-powerpoint"},
                                   {"py", "text/x-script.phyton"},
                                   {"pyc", "applicaiton/x-bytecode.python"},
                                   {"qcp", "audio/vnd.qcelp"},
                                   {"qd3", "x-world/x-3dmf"},
                                   {"qd3d", "x-world/x-3dmf"},
                                   {"qif", "image/x-quicktime"},
                                   {"qt", "video/quicktime"},
                                   {"qtc", "video/x-qtc"},
                                   {"qti", "image/x-quicktime"},
                                   {"qtif", "image/x-quicktime"},
                                   {"ra", "audio/x-pn-realaudio"},
                                   {"ram", "audio/x-pn-realaudio"},
                                   {"ras", "application/x-cmu-raster"},
                                   {"rast", "image/cmu-raster"},
                                   {"rexx", "text/x-script.rexx"},
                                   {"rf", "image/vnd.rn-realflash"},
                                   {"rgb", "image/x-rgb"},
                                   {"rm", "application/vnd.rn-realmedia"},
                                   {"rmi", "audio/mid"},
                                   {"rmm", "audio/x-pn-realaudio"},
                                   {"rmp", "audio/x-pn-realaudio"},
                                   {"rng", "application/ringing-tones"},
                                   {"rnx", "application/vnd.rn-realplayer"},
                                   {"roff", "application/x-troff"},
                                   {"rp", "image/vnd.rn-realpix"},
                                   {"rpm", "audio/x-pn-realaudio-plugin"},
                                   {"rt", "text/richtext"},
                                   {"rtf", "text/richtext"},
                                   {"rtx", "application/rtf"},
                                   {"rv", "video/vnd.rn-realvideo"},
                                   {"s", "text/x-asm"},
                                   {"s3m", "audio/s3m"},
                                   {"saveme", "application/octet-stream"},
                                   {"sbk", "application/x-tbook"},
                                   {"scm", "application/x-lotusscreencam"},
                                   {"sdml", "text/plain"},
                                   {"sdp", "application/sdp"},
                                   {"sdr", "application/sounder"},
                                   {"sea", "application/sea"},
                                   {"set", "application/set"},
                                   {"sgm", "text/sgml"},
                                   {"sgml", "text/sgml"},
                                   {"sh", "application/x-bsh"},
                                   {"shtml", "text/html"},
                                   {"sid", "audio/x-psid"},
                                   {"sit", "application/x-sit"},
                                   {"skd", "application/x-koan"},
                                   {"skm", "application/x-koan"},
                                   {"skp", "application/x-koan"},
                                   {"skt", "application/x-koan"},
                                   {"sl", "application/x-seelogo"},
                                   {"smi", "application/smil"},
                                   {"smil", "application/smil"},
                                   {"snd", "audio/basic"},
                                   {"sol", "application/solids"},
                                   {"spc", "application/x-pkcs7-certificates"},
                                   {"spl", "application/futuresplash"},
                                   {"spr", "application/x-sprite"},
                                   {"sprite", "application/x-sprite"},
                                   {"src", "application/x-wais-source"},
                                   {"ssi", "text/x-server-parsed-html"},
                                   {"ssm", "application/streamingmedia"},
                                   {"sst", "application/vnd.ms-pki.certstore"},
                                   {"step", "application/step"},
                                   {"stl", "application/sla"},
                                   {"stp", "application/step"},
                                   {"sv4cpio", "application/x-sv4cpio"},
                                   {"sv4crc", "application/x-sv4crc"},
                                   {"svf", "image/vnd.dwg"},
                                   {"svr", "application/x-world"},
                                   {"swf", "application/x-shockwave-flash"},
                                   {"t", "application/x-troff"},
                                   {"talk", "text/x-speech"},
                                   {"tar", "application/x-tar"},
                                   {"tbk", "application/toolbook"},
                                   {"tcl", "application/x-tcl"},
                                   {"tcsh", "text/x-script.tcsh"},
                                   {"tex", "application/x-tex"},
                                   {"texi", "application/x-texinfo"},
                                   {"texinfo", "application/x-texinfo"},
                                   {"text", "text/plain"},
                                   {"tgz", "application/x-compressed"},
                                   {"tif", "image/tiff"},
                                   {"tr", "application/x-troff"},
                                   {"tsi", "audio/tsp-audio"},
                                   {"tsp", "audio/tsplayer"},
                                   {"tsv", "text/tab-separated-values"},
                                   {"turbot", "image/florian"},
                                   {"txt", "text/plain"},
                                   {"uil", "text/x-uil"},
                                   {"uni", "text/uri-list"},
                                   {"unis", "text/uri-list"},
                                   {"unv", "application/i-deas"},
                                   {"uri", "text/uri-list"},
                                   {"uris", "text/uri-list"},
                                   {"ustar", "application/x-ustar"},
                                   {"uu", "application/octet-stream"},
                                   {"vcd", "application/x-cdlink"},
                                   {"vcs", "text/x-vcalendar"},
                                   {"vda", "application/vda"},
                                   {"vdo", "video/vdo"},
                                   {"vew", "application/groupwise"},
                                   {"viv", "video/vivo"},
                                   {"vivo", "video/vivo"},
                                   {"vmd", "application/vocaltec-media-desc"},
                                   {"vmf", "application/vocaltec-media-file"},
                                   {"voc", "audio/voc"},
                                   {"vos", "video/vosaic"},
                                   {"vox", "audio/voxware"},
                                   {"vqe", "audio/x-twinvq-plugin"},
                                   {"vqf", "audio/x-twinvq"},
                                   {"vql", "audio/x-twinvq-plugin"},
                                   {"vrml", "application/x-vrml"},
                                   {"vrt", "x-world/x-vrt"},
                                   {"vsd", "application/x-visio"},
                                   {"vst", "application/x-visio"},
                                   {"vsw", "application/x-visio"},
                                   {"w60", "application/wordperfect6.0"},
                                   {"w61", "application/wordperfect6.1"},
                                   {"w6w", "application/msword"},
                                   {"wav", "audio/wav"},
                                   {"wb1", "application/x-qpro"},
                                   {"wbmp", "image/vnd.wap.wbmp"},
                                   {"web", "application/vnd.xara"},
                                   {"wiz", "application/msword"},
                                   {"wk1", "application/x-123"},
                                   {"wmf", "windows/metafile"},
                                   {"wml", "text/vnd.wap.wml"},
                                   {"wmlc", "application/vnd.wap.wmlc"},
                                   {"wmls", "text/vnd.wap.wmlscript"},
                                   {"wmlsc", "application/vnd.wap.wmlscriptc"},
                                   {"word", "application/msword"},
                                   {"wp", "application/wordperfect"},
                                   {"wp5", "application/wordperfect"},
                                   {"wp6", "application/wordperfect"},
                                   {"wpd", "application/wordperfect"},
                                   {"wq1", "application/x-lotus"},
                                   {"wri", "application/mswrite"},
                                   {"wrl", "application/x-world"},
                                   {"wrz", "model/vrml"},
                                   {"wsc", "text/scriplet"},
                                   {"wsrc", "application/x-wais-source"},
                                   {"wtk", "application/x-wintalk"},
                                   {"xbm", "image/x-xbitmap"},
                                   {"xdr", "video/x-amt-demorun"},
                                   {"xgz", "xgl/drawing"},
                                   {"xif", "image/vnd.xiff"},
                                   {"xl", "application/excel"},
                                   {"xla", "application/excel"},
                                   {"xlb", "application/excel"},
                                   {"xlc", "application/excel"},
                                   {"xld", "application/excel"},
                                   {"xlk", "application/excel"},
                                   {"xll", "application/excel"},
                                   {"xlm", "application/excel"},
                                   {"xls", "application/excel"},
                                   {"xlt", "application/excel"},
                                   {"xlv", "application/excel"},
                                   {"xlw", "application/excel"},
                                   {"xm", "audio/xm"},
                                   {"xml", "text/xml"},
                                   {"xmz", "xgl/movie"},
                                   {"xpix", "application/x-vnd.ls-xpix"},
                                   {"xpm", "image/x-xpixmap"},
                                   {"x-png", "image/png"},
                                   {"xsr", "video/x-amt-showrun"},
                                   {"xwd", "image/x-xwd"},
                                   {"xyz", "chemical/x-pdb"},
                                   {"z", "application/x-compress"},
                                   {"zip", "application/x-compressed"},
                                   {"zoo", "application/octet-stream"},
                                   {"zsh", "text/x-script.zsh"}
                               };
        }
        public static string SubstringAfter(this string source, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return source;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.Ordinal);
            if (index < 0)
            {
                //No such substring
                return string.Empty;
            }
            return source[(index + value.Length)..];
        }

        public static string SubstringBefore(this string source, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int index = compareInfo.IndexOf(source, value, CompareOptions.Ordinal);
            if (index < 0)
            {
                //No such substring
                return string.Empty;
            }
            return source.Substring(0, index);
        }

        public static string Encrypt(string plainText)
        {
            //Take Project Name As Salt Value 
            return Encrypt(plainText, "T1mE|1NE", "TMAS", "SHA1", 5, "@1B2c3D4e5F6g7H8", 256);
        }

        private static string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }
        public static string Decrypt(string cipherText)
        {
            //Take Project Name As Salt Value 
            return Decrypt(cipherText, "T1mE|1NE", "TMAS", "SHA1", 5, "@1B2c3D4e5F6g7H8", 256);
        }
        private static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length - 1);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }
        public static string RandomAlphaNumericString(int len)
        {
            int maxSize = len;
            char[] chars = new char[30];
            string a;
            a = "abcdefghijklmnopqrstuvwxyz1234567890";
            chars = a.ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            int size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data) { result.Append(chars[b % (chars.Length)]); }
            return result.ToString();
        }

        public static void UpdateRequestedFilters(in SearchFilter filters, string paggingConfig)
        {
            filters.Offset = filters.Offset;
            filters.PageSize = filters.PageSize == 0 ? Convert.ToInt32(paggingConfig) : filters.PageSize;

            if (filters.sortFilter != null)
            {
                filters.sortFilter.SortBy = String.IsNullOrEmpty(filters.sortFilter?.SortBy) ? "CreatedOn" : filters.sortFilter?.SortBy;
                filters.sortFilter.Direction = String.IsNullOrEmpty(filters.sortFilter?.Direction) ? "DESC" : filters.sortFilter?.Direction;
            }
        }
    }
}
