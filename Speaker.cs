using System;
using System.Collections.Generic;
using System.Linq;

namespace GK.Talks
{
	/// <summary>
	/// Represents a single speaker
	/// </summary>
	/// </summary>
	public class Speaker(IRepository repository)
	{
		private readonly IRepository _repository = repository;
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Exp { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// Prams 
		/// strFirstName speakers first name
		///	strLastName ^^^ last name
		/// Email the email
		/// blogs etc.....
		/// </summary>
		/// <returns>speakerID</returns>
		public RegisterResponse Register(string strFirstName, String strLastName, string Email, int iExp, Boolean BHasBlog, string URL, string strBrowser, string csvCertifications, String s_Emp, int iFee, string csvSess)
		{
			int? speakerId = null;

			if (string.IsNullOrWhiteSpace(FirstName))
				return new RegisterResponse(RegisterError.FirstNameRequired);

			if (string.IsNullOrWhiteSpace(LastName))
				return new RegisterResponse(RegisterError.LastNameRequired);

			if (string.IsNullOrWhiteSpace(Email))
				return new RegisterResponse(RegisterError.EmailRequired);

			if (Sessions == null || Sessions.Count == 0)
				return new RegisterResponse(RegisterError.NoSessionsProvided);

			if (!IsSpeakerQualified())
				return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);

			if (!ApproveSessions(Sessions))
				return new RegisterResponse(RegisterError.NoSessionsApproved);

			RegistrationFee = CalculateRegistrationFee();

			try
			{
				speakerId = _repository.SaveSpeaker(this);
			}
			catch (Exception)
			{
				return new RegisterResponse(RegisterError.DatabaseError);
			}

			return new RegisterResponse((int)speakerId);
		}

		private bool IsSpeakerQualified()
		{
			var emps = new List<string> { "Pluralsight", "Microsoft", "Google" };
			var domains = new List<string> { "aol.com", "prodigy.com", "compuserve.com" };

			if (Exp > 10 || HasBlog || (Certifications != null && Certifications.Count > 3) || emps.Contains(Employer))
				return true;

			if (!string.IsNullOrWhiteSpace(Email))
			{
				string emailDomain = Email.Split('@').Last();
				if (domains.Contains(emailDomain))
					return false;
			}

			if (Browser != null && Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)
				return false;

			return true;
		}

		private bool ApproveSessions(List<Session> sessions)
		{
			var ot = new List<string> { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			bool anyApproved = false;

			foreach (var session in sessions)
			{
				bool rejected = ot.Any(tech => session.Title.Contains(tech) || session.Description.Contains(tech));
				if (rejected)
				{
					session.Approved = false;
				}
				else
				{
					session.Approved = true;
					anyApproved = true;
				}
			}

			return anyApproved;
		}

		private int CalculateRegistrationFee()
		{
			if (Exp <= 1)
				return 500;
			if (Exp >= 2 && Exp <= 3)
				return 250;
			if (Exp >= 4 && Exp <= 5)
				return 100;
			if (Exp >= 6 && Exp <= 9)
				return 50;
			return 0;
		}
	}
}