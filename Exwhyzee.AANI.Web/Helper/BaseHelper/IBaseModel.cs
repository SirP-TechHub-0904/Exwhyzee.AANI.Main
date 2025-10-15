using Exwhyzee.AANI.Domain.Models;

namespace Exwhyzee.AANI.Web.Helper.BaseHelper
{
    public interface IBaseModel
    {
        Task<List<PageSection>> GetPageSections(string? position, string? pageset, long? id, long? secid);
        Task<MenuDataDto> GetMenuData();
        Task<FooterMenuDataDto> GetFooterMenuDataAsync();
        Task<ConfigDto> GetConfigAsync();
    }
}
