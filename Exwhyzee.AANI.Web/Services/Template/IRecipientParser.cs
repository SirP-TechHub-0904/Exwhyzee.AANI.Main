using System.Text.RegularExpressions;

namespace Exwhyzee.AANI.Web.Services.Template
{
    public interface IRecipientParser
    {
        // Parses manual recipient input and returns a list of recipients (valid and invalid entries).
        ParseResult Parse(string input);
    }

    public class RecipientDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Error { get; set; }
    }

    public class ParseResult
    {
        public List<RecipientDto> Recipients { get; set; } = new List<RecipientDto>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class RecipientParser : IRecipientParser
    {
        // Accepts format:
        // (fullname,email,phone)(fullname,email,phone) OR one-per-line csv: fullname,email,phone
        // Lenient parsing: missing email or phone allowed but flagged.
        private static readonly Regex PackRegex = new Regex(@"\(\s*([^\)]+?)\s*\)", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneDigitRegex = new Regex(@"\d", RegexOptions.Compiled);

        public ParseResult Parse(string input)
        {
            var result = new ParseResult();
            if (string.IsNullOrWhiteSpace(input)) return result;

            // First try to find packed groups ( (a,b,c)(d,e,f) )
            var matches = PackRegex.Matches(input);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    var content = m.Groups[1].Value;
                    var parts = SplitCsv(content);
                    var dto = NormalizeParts(parts);
                    result.Recipients.Add(dto);
                }
                return result;
            }

            // Otherwise treat as lines (each line a CSV)
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(l => l.Trim())
                             .Where(l => !string.IsNullOrEmpty(l));

            foreach (var line in lines)
            {
                var parts = SplitCsv(line);
                var dto = NormalizeParts(parts);
                result.Recipients.Add(dto);
            }

            return result;
        }

        private static string[] SplitCsv(string line)
        {
            // simple split on comma - this is reasonable for the expected input
            return line.Split(',').Select(p => p.Trim()).ToArray();
        }

        private static bool LooksLikeEmail(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return EmailRegex.IsMatch(s);
        }

        private static bool LooksLikePhone(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            // Consider it a phone if it contains at least one digit and mostly digits/space/+-() characters
            // This is permissive: +234 801 234 5678, 08012345678, (080)123-4567 etc.
            var trimmed = s.Trim();
            // quick check: contains digit
            if (!PhoneDigitRegex.IsMatch(trimmed)) return false;
            // check allowed characters
            var allowed = Regex.IsMatch(trimmed, @"^[\d\+\-\s\(\)]+$");
            return allowed;
        }

        private static RecipientDto NormalizeParts(string[] parts)
        {
            var dto = new RecipientDto();

            // Behavior:
            // - Single token:
            //     - if it looks like phone -> phone
            //     - else if looks like email -> email
            //     - else -> fullname
            // - Two tokens:
            //     - common case: fullname,email OR fullname,phone
            //     - if first looks like phone and second not -> treat first as phone second as name
            // - Three or more tokens:
            //     - fullname,email,phone (first three used)
            // Trim values always.

            if (parts.Length == 1)
            {
                var p0 = parts[0];
                if (LooksLikePhone(p0))
                {
                    dto.Phone = p0;
                }
                else if (LooksLikeEmail(p0))
                {
                    dto.Email = p0;
                }
                else
                {
                    dto.FullName = p0;
                }
            }
            else if (parts.Length == 2)
            {
                var p0 = parts[0];
                var p1 = parts[1];

                // if second looks like email or phone, first is likely fullname
                if (LooksLikeEmail(p1))
                {
                    dto.FullName = p0;
                    dto.Email = p1;
                }
                else if (LooksLikePhone(p1))
                {
                    dto.FullName = p0;
                    dto.Phone = p1;
                }
                else if (LooksLikePhone(p0) && !LooksLikePhone(p1))
                {
                    // possible "phone,fullname"
                    dto.Phone = p0;
                    dto.FullName = p1;
                }
                else if (LooksLikeEmail(p0) && !LooksLikeEmail(p1))
                {
                    // possible "email,fullname" unlikely but handle
                    dto.Email = p0;
                    dto.FullName = p1;
                }
                else
                {
                    // fallback: treat as fullname,email (common user case)
                    dto.FullName = p0;
                    dto.Email = p1;
                }
            }
            else // 3 or more
            {
                dto.FullName = parts[0];
                dto.Email = LooksLikeEmail(parts[1]) ? parts[1] : parts[1];
                dto.Phone = parts.Length >= 3 ? parts[2] : null;
            }

            // Basic validation: at least one contact (email or phone) must exist
            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
            {
                // keep the fullname but mark error
                dto.Error = "Missing both email and phone";
            }

            return dto;
        }
    }
}









//using System.Text.RegularExpressions;

//namespace Exwhyzee.AANI.Web.Services.Template
//{
//    public interface IRecipientParser
//    {
//        // Parses manual recipient input and returns a list of recipients (valid and invalid entries).
//        ParseResult Parse(string input);
//    }

//    public class RecipientDto
//    {
//        public string FullName { get; set; } = string.Empty;
//        public string? Email { get; set; }
//        public string? Phone { get; set; }
//        public string? Error { get; set; }
//    }

//    public class ParseResult
//    {
//        public List<RecipientDto> Recipients { get; set; } = new List<RecipientDto>();
//        public List<string> Errors { get; set; } = new List<string>();
//    }

//    public class RecipientParser : IRecipientParser
//    {
//        // Accepts format:
//        // (fullname,email,phone)(fullname,email,phone) OR one-per-line csv: fullname,email,phone
//        // Lenient parsing: missing email or phone allowed but flagged.
//        private static readonly Regex PackRegex = new Regex(@"\(\s*([^\)]+?)\s*\)", RegexOptions.Compiled);

//        public ParseResult Parse(string input)
//        {
//            var result = new ParseResult();
//            if (string.IsNullOrWhiteSpace(input)) return result;

//            // First try to find packed groups ( (a,b,c)(d,e,f) )
//            var matches = PackRegex.Matches(input);
//            if (matches.Count > 0)
//            {
//                foreach (Match m in matches)
//                {
//                    var content = m.Groups[1].Value;
//                    var parts = SplitCsv(content);
//                    var dto = NormalizeParts(parts);
//                    result.Recipients.Add(dto);
//                }
//                return result;
//            }

//            // Otherwise treat as lines (each line a CSV)
//            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
//                             .Select(l => l.Trim())
//                             .Where(l => !string.IsNullOrEmpty(l));

//            foreach (var line in lines)
//            {
//                var parts = SplitCsv(line);
//                var dto = NormalizeParts(parts);
//                result.Recipients.Add(dto);
//            }

//            return result;
//        }

//        private static string[] SplitCsv(string line)
//        {
//            // simple split on comma - this is reasonable for the expected input
//            return line.Split(',').Select(p => p.Trim()).ToArray();
//        }

//        private static RecipientDto NormalizeParts(string[] parts)
//        {
//            var dto = new RecipientDto();
//            if (parts.Length >= 1) dto.FullName = parts[0];
//            if (parts.Length >= 2) dto.Email = string.IsNullOrWhiteSpace(parts[1]) ? null : parts[1];
//            if (parts.Length >= 3) dto.Phone = string.IsNullOrWhiteSpace(parts[2]) ? null : parts[2];

//            // Basic validation: at least one contact (email or phone) must exist
//            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
//            {
//                dto.Error = "Missing both email and phone";
//            }

//            return dto;
//        }
//    }
//}
