using System;
using System.Collections.Generic;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions.Extras;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Identity;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions
{
    
    public class Competition
    {
        
        public Guid Id { get; set; }
        
        public Visibility VisibleTo { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        
        public bool PinCodeEnabled { get; set; }
        public string PinCodeClearText { get; set; }
        
        public bool UserExitEnabled { get; set; }
        public CompetitionExitAction UserExitAction { get; set; }
        
        public bool EnableWaitingPage { get; set; }
        public DateTime WaitingPageActivationTime { get; set; }
        
        /*
         * Relationships
         */
        
        public User Curator { get; set; }
        public Guid CuratorId { get; set; }
        
        public List<CompetitionProgrammingTask> CompetitionProgrammingTasks { get; set; }
        
        public List<CompetitionUser> CompetitionUsers { get; set; }
        
        public enum CompetitionExitAction
        {
            Default,
            DeleteTestingApplications
        }
        
        /*
         * >>> Тлумачення значень Visibility:
         * 
         *  -  SupervisedUsers - змагання доступне лише користувачам,
         *     куратором яких є автор змагання;
         * 
         *  -  SubSupervisedUsers - змагання доступне лише користувачам,
         *     кураторами яких є користувачі, куратором яких є куратор змагання;
         * 
         *  -  AllUsers - змагання доступне усім користувачам системи;
         *
         *  -  Hidden - тільки куратор змагання може запрошувати користувачів до нього;
         */
        
        public enum Visibility
        {
            SupervisedUsers,
            SubSupervisedUsers,
            AllUsers,
            Hidden
        }
        
    }
    
}