namespace JabbR.ContentProviders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using JabbR.ContentProviders.Core;
    using JabbR.Tfs;

    public class TfsContentProvider : CollapsibleContentProvider
    {
        public override bool IsValidContent(Uri uri)
        {
            if (uri.Host.Contains("dvc-tfs12") && uri.AbsoluteUri.Contains("_workitems#"))
            {
                return true;    
            }
            return false;
        }

        protected override Task<ContentProviderResult> GetCollapsibleContent(ContentProviderHttpRequest request)
        {
            var parameters = new System.Collections.Specialized.StringDictionary();
            foreach (var pv in request.RequestUri.Fragment.Replace("#", "").Split('&').Select(paramValue => paramValue.Split('=')).Where(pv => pv.Length == 2))
            {
                parameters.Add(pv[0], pv[1]);
            }
            
            // _workitems#_a=edit&id=29141
            if (request.RequestUri.AbsoluteUri.Contains("_workitems#"))
            {
                int id;
                if (parameters.ContainsKey("id") && int.TryParse(parameters["id"], out id))
                {
                    return TaskAsyncHelper.FromResult(
                        new ContentProviderResult
                        {
                            Title = "TFS",
                            Content = new TfsToHtml().GetTaskHtml(id)
                        });
                }
            }

            return null;
        }

    }
}