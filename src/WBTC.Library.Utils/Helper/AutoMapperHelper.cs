using AutoMapper;
using SimpleWifi;
using WBTC.Library.Utils.Model;

namespace WBTC.Library.Utils.Helper
{
    public class AutoMapperHelper
    {
        static MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<AccessPoint, WifiModel>().ReverseMap();
        });
        public static IMapper mapper = configuration.CreateMapper();
    }
}
