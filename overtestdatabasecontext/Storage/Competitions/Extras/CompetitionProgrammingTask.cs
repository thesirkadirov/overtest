using System;
using Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TasksArchive;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.Competitions.Extras
{
    
    public class CompetitionProgrammingTask
    {
        
        public Competition Competition { get; set; }
        public Guid CompetitionId { get; set; }
        
        public ProgrammingTask ProgrammingTask { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        
        public ProgrammingTaskJudgementType JudgementType { get; set; }

        public enum ProgrammingTaskJudgementType
        {
            PartialSolution,
            CompleteSolution
        }
        
    }
    
}