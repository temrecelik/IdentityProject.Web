using IdentityProject.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Data;
using System.Text;

namespace IdentityProject.Web.TagHelpers
{
	/*
	 TagHelperlar html sayfalarında business kodları yazmamak için kullanılır.Bu tag helper userlist'deki userların
		rollerini bir rozet olarak göstermek için yazılacaktır.
	 */
	 
	public class UserRoleNamesTagHelper : TagHelper
	{
		public string UserId { get; set; }	
		private readonly  UserManager<User> _userManager;

		public UserRoleNamesTagHelper(UserManager<User> userManager)
		{
			_userManager = userManager;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var user = await _userManager.FindByIdAsync(UserId);
			var userRoles =await _userManager.GetRolesAsync(user!);

		
			var stringBuilder = new StringBuilder();

			if (userRoles.Any())
			{
				
					userRoles.ToList().ForEach(role =>
					{
						stringBuilder.Append($"<span class='badge bg-secondary mx-1'>{role.ToLower()}</span>");

					});
					

            }
            else
			{
                stringBuilder.Append("Rol Bulunamadı");
            }
			output.Content.SetHtmlContent(stringBuilder.ToString());	
		}
	}
}
