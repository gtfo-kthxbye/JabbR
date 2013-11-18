using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JabbR.Tfs
{
    using System.Net;

    using Newtonsoft.Json;

    public class TfsToHtml
    {
        private const string tfsUri = "http://dvc-tfs12:8080/tfs";
        private const string tfsCollectionUri = "http://dvc-tfs12:8080/tfs/DefaultCollection";

        public string GetWorkItemHtml(int taskId)
        {
            try
            {
                // workItem json url pattern
                // http://dvc-tfs12:8080/tfs/DefaultCollection/_api/_wit/workitems?__v=3&ids=31189                
                var apiUrl = string.Format("{0}/_api/_wit/workitems?__v=3&ids={1}", tfsCollectionUri, taskId);
                string json = string.Empty;
                var dom = "DEVCORE";
                var l = "maand";
                var pas = "#buraki123!@";                                
                using (var webClient = new System.Net.WebClient()) 
                {
                    webClient.Credentials = new System.Net.NetworkCredential(l, pas, dom);
                    webClient.Encoding = UTF8Encoding.UTF8;
                    json = webClient.DownloadString(apiUrl);                    
                }

                var o = JsonConvert.DeserializeObject<dynamic>(json);
                if (o == null || o.__wrappedArray.Length == 0)
                {
                    
                }

                // image url pattern
                // http://dvc-tfs12:8080/tfs/DefaultCollection/_api/_wit/DownloadAttachment?fileName=1426749_316968105110623_1093586580_n.jpg&fileGuid=1f6bf377-0060-4b3c-a572-12e9efa23da7&contentOnly=true&__v=3
                var att = new List<dynamic>();
                foreach (var file in o.__wrappedArray[0].files)
                {
                    att.Add(new
                    {
                        OriginalName = file.OriginalName.Value,
                        Ext = System.IO.Path.GetExtension(file.OriginalName.Value),
                        FilePath = file.FilePath.Value,
                        Url = string.Format("{0}/_api/_wit/DownloadAttachment?fileName={1}&fileGuid={2}&contentOnly=true&__v=3", tfsCollectionUri, file.OriginalName.Value, file.FilePath.Value)
                    });
                }

                var workItem = new
                                   {
                                       TypeName = (string)this.GetValue(o.__wrappedArray[0].fields["25"]),
                                       Title = this.GetValue(o.__wrappedArray[0].fields["1"]),
                                       ProjectName = this.GetValue(o.__wrappedArray[0].fields["-7"]),
                                       CreatedBy = this.GetValue(o.__wrappedArray[0].fields["10005"]),
                                       AssignedTo = this.GetValue(o.__wrappedArray[0].fields["24"]),
                                       Description = this.GetValue(o.__wrappedArray[0].fields["52"]),
                                       Description2 = this.GetValue(o.__wrappedArray[0].fields["10002"]),                                    
                                       Attachments = att.OrderBy(x => x.Ext).ThenBy(x => x.OriginalName).ToList()
                                   };


                string color = "#B000CF";
                switch (workItem.TypeName)
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
                switch (workItem.TypeName)
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
                                if (a.OriginalName.EndsWith("jpg") || a.OriginalName.EndsWith("png") || a.OriginalName.EndsWith("jpeg") || a.OriginalName.EndsWith("gif") || a.OriginalName.EndsWith("bmp"))
                                {
                                    //customHtml += string.Format("<a href=\"{0}\" class=\"lytebox\" data-lyte-options=\"group:{1}\" data-title=\"{2}\"><img style=\"max-height: 150px; margin: 2px; border: solid 1px gray; box-shadow: 3px 3px 10px gray;\" src=\"{0}\"/></a>", a.Url, taskId, workItem.Title);
                                    customHtml += string.Format("<a href=\"{0}\" target=\"_blank\" class=\"fancybox\" rel=\"workItem{1}\" title=\"{2}\"><img style=\"max-height: 150px; margin: 2px; border: solid 1px gray; box-shadow: 3px 3px 10px gray;\" src=\"{0}\"/></a>", a.Url, taskId, workItem.Title);
                                }
                                else
                                {
                                    customHtml += string.Format("<a style=\"margin: 5px; color: #000;\" href=\"{0}\" target=\"_blank\">{1}</a>", a.Url, a.OriginalName);
                                }
                                
                            }
                        }
                        break;
                    case "Historia pracy":
                        break;
                }
                var image = tfsUri + "/_static/tfs/11/_content/Header/tfserver2012.png";
                var url = tfsCollectionUri + string.Format("/{0}/_workItems#_a=edit&id={1}", workItem.ProjectName, taskId);

                const string html =
                    "<div style=\"background-color: {0}; padding: 5px;\">"
                    + "<img src=\"{1}\"/>"
                    + "<h3><a style=\"color: #fff;\" href=\"{2}\" target=\"_blank\">{3}</a></h3>"
                    + "<div style=\"padding: 5px; background-color: rgb(255, 255, 255);\">utworzył: {4}, przypisany do: <strong>{5}</strong> <p>{6}</p> {7}</div>"
                    + "</div>";

                return string.Format(html, color, image, url, workItem.Title, workItem.CreatedBy, workItem.AssignedTo, workItem.Description + workItem.Description2, customHtml);
            }
                catch (Exception ex)
                {
                    var err = ex.Message;
                    if (ex.InnerException != null)
                    {
                        err += ex.InnerException;
                    }
                    return err;
                }
            }

        private string GetValue(dynamic o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            else
            {
                return (string)o.Value;
            }
        }
    }
}
