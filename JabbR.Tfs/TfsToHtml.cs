using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JabbR.Tfs
{
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public class TfsToHtml
    {
        private const string tfsUri = "http://dvc-tfs12:8080/tfs";
        private const string tfsCollectionUri = "http://dvc-tfs12:8080/tfs/DefaultCollection";

        public string GetTaskHtml(int taskId)
        {
            var collectionUri = new Uri(tfsCollectionUri);
            var tpc = new TfsTeamProjectCollection(collectionUri);
            var workItemStore = tpc.GetService<WorkItemStore>();
            var workItem = workItemStore.GetWorkItem(taskId);

            string color = "#B000CF";            
            switch (workItem.Type.Name)
            {
                case "Task":
                    color = "#007ACC";
                    break;
                case "Bug":
                    color = "#EF002A";
                    break;
                case "User Story":
                    color = "#FF8A00";
                    break;
            }

            string customHtml = string.Empty;
            switch (workItem.Type.Name)
            {
                case "Task":
                case "Bug":
                case "User Story":
                    if (workItem.Attachments.Count > 0)
                    {
                        for (int i = 0; i < workItem.Attachments.Count; i++)
                        {
                            var a = workItem.Attachments[i];
                            var name = string.Empty;
                            if (a.Extension.EndsWith("jpg") || a.Extension.EndsWith("png") || a.Extension.EndsWith("jpeg") || a.Extension.EndsWith("gif") || a.Extension.EndsWith("bmp"))
                            {
                                name = string.Format("<img style=\"max-height: 150px;\" src=\"{0}\"/>", a.Uri.AbsoluteUri);
                            }
                            else
                            {
                                name = a.Name;
                            }
                            customHtml += string.Format("<a style=\"margin: 5px; color: #000;\" href=\"{0}\" target=\"_blank\">{1}</a>", a.Uri.AbsoluteUri, name);
                        }
                    }
                    break;                
                case "Historia pracy":
                    break;
            }
            var image = tfsUri + "/_static/tfs/11/_content/Header/tfserver2012.png";
            var url = tfsCollectionUri + string.Format("/{0}/_workItems#_a=edit&id={1}", workItem.Project.Name, taskId);

            const string html = 
                "<div style=\"background-color: {0}; padding: 5px;\">"
                + "<img src=\"{1}\"/>"
                + "<h3><a style=\"color: #fff;\" href=\"{2}\" target=\"_blank\">{3}</a></h3>"
                + "<div style=\"padding: 5px; background-color: rgb(255, 255, 255);\">utworzył: {4} <p>{5}</p> {6}</div>" 
                + "</div>";
            
            return string.Format(html, color, image, url, workItem.Title, workItem.CreatedBy, workItem.Description, customHtml);
        }

    }
}
