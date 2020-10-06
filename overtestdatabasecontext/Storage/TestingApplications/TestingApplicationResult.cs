using System;
using System.ComponentModel.DataAnnotations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Storage.TestingApplications
{
    public class TestingApplicationResult
    {
        public Guid Id { get; set; }
        
        public TimeSpan ProcessingTime { get; set; }
        public string RawTestingResults { get; set; }
            
        [Range(typeof(byte), "0", "100")]
        public byte GivenDifficulty { get; set; }

        public SolutionAdjudgementType SolutionAdjudgement { get; set; }
        
        public TestingApplication TestingApplication { get; set; }
        public Guid TestingApplicationId { get; set; }
        
        public enum SolutionAdjudgementType
        {
            ZeroSolution = 0,
            PartialSolution = 50,
            CompleteSolution = 100
        }
    }
}