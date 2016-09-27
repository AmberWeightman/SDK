using RobotAPISample.Automation;
using RobotAPISample.RequestsResponses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotAPISample.Workflows
{
    public enum LINZTitleSearchType
    {
        TitleSearchWithDiagram,
        TitleSearchNoDiagram,
        Historical,
        Guaranteed,
    }

    public class LINZTitleSearchWorkflowRequest : WorkflowSearchRequest
    {
        // Use arrays to enforce order between TitleReferences, Types and OrderIds
        public string[] TitleReferences { get; set; }

        public LINZTitleSearchType[] Types { get; set; }
        
        public TitleSearchRequest[] SourceRequests { get; }

        public LINZTitleSearchWorkflowRequest(TitleSearchRequest[] titleSearchRequests)
        {
            SourceRequests = titleSearchRequests;

            var requestCount = titleSearchRequests.Count();
            var titleReferences = new string[requestCount];
            var types = new LINZTitleSearchType[requestCount];
            var orderIds = new string[requestCount];

            for (var i = 0; i < requestCount; i++)
            {
                titleReferences[i] = titleSearchRequests[i].TitleReference;
                types[i] = titleSearchRequests[i].Type;
                orderIds[i] = titleSearchRequests[i].OrderId;
            }

            TitleReferences = titleReferences;
            Types = types;
            OrderIds = orderIds;
        }

        public override bool Validate()
        {
            var isValid = base.Validate();

            if (TitleReferences == null || !TitleReferences.Any())
            {
                throw new ApplicationException("At least one title reference is required.");
            }
            if (TitleReferences.Count() != OrderIds.Count())
            {
                throw new ApplicationException("Mismatched OrderId/TitleReference count.");
            }
            if (Types.Count() != OrderIds.Count())
            {
                throw new ApplicationException("Mismatched OrderId/Type count.");
            }

            return isValid;
        }
    }

    public class LINZTitleSearchWorkflowResponse : WorkflowSearchResponse
    {
        public List<SearchResult> SearchResults { get; set; }

        public List<string> FileNames { get; set; }

        public override bool Validate(IWorkflowRequest request)
        {
            var isValid = base.Validate(request);

            var workflowSearchRequest = request as LINZTitleSearchWorkflowRequest;
            if (workflowSearchRequest != null)
            {
                if (FileNames != null && FileNames.Any())
                {
                    // Check that the files actually exist, because we can't trust the workflow for anything
                    foreach (var fileName in FileNames)
                    {
                        var filePath = $"{WorkflowSearchRequest.OutputDirectory}{fileName}";
                        if (!File.Exists(filePath))
                        {
                            throw new ApplicationException($"Failed to successfully create file {filePath}.");
                        }

                        // Update the source request with the saved file name
                        var orderId = fileName.Split('_')[0];
                        var associatedSourceRequest = workflowSearchRequest.SourceRequests.SingleOrDefault(sr => string.Equals(sr.OrderId, orderId));
                        if (associatedSourceRequest != null)
                        {
                            associatedSourceRequest.Success = true;
                            associatedSourceRequest.OutputFilePath = filePath;
                        }
                    }
                }
            }
            
            return isValid;
        }
    }

    public class SearchResult
    {
        public string CT { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public string PotentiallyMaoriLand { get; set; }
        public string LegalDescription { get; set; }
        public string IndicativeArea { get; set; }
        public string LandDistrict { get; set; }
        public string TimeshareWeek { get; set; }

        public SearchResult(string tabSeparatedData)
        {
            var lines = tabSeparatedData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Any()) // for now, assumes there is only one line (or two lines, with the first line being the header)
            {
                var values = lines.Last().Split(new char[] { '\t' }, StringSplitOptions.None);
                CT = values[0];
                Owner = values[1];
                Status = values[2];
                PotentiallyMaoriLand = values[3];
                LegalDescription = values[4];
                IndicativeArea = values[5];
                LandDistrict = values[6];
                TimeshareWeek = values[7];
            }
        }
        
    }
    
    public class LINZTitleSearchWorkflow : RobotWorkflowBase<LINZTitleSearchWorkflowRequest, LINZTitleSearchWorkflowResponse>
    {
        public  override string WorkflowFile => WorkflowFiles.LINZTitleSearchWorkflow;

        public override WorkflowType WorkflowType => WorkflowType.LINZTitleSearch;

        public override int MaxWorkflowDurationMins => 10;
    }
    
}
