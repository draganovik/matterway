using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace SharedProject.Profiles;

public class ValidationProfile : Profile
{
    public class ValidationResultProfile : Profile
    {
        public ValidationResultProfile()
        {
            CreateMap<List<ValidationResult>, Dictionary<string, string>>()
                .ConvertUsing<ValidationResultToDictionaryConverter>();
        }
    }

    public class ValidationResultToDictionaryConverter : ITypeConverter<List<ValidationResult>, Dictionary<string, string>>
    {
        public Dictionary<string, string> Convert(List<ValidationResult> source, Dictionary<string, string> destination, ResolutionContext context)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var validationResult in source)
            {
                foreach (var member in validationResult.MemberNames)
                {
                    char[] memberName = member.ToCharArray();
                    memberName[0] = char.ToLower(member[0]);
                    dictionary.Add(new string(memberName) ?? "Unknown", validationResult.ErrorMessage ?? "Unknown");
                }
            }
            return dictionary;
        }
    }

}
