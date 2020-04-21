using HelpMyStreetFE.Models.RequestHelp;


namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
		void PersistForm(RequestHelpFormModel form);
	}
}
