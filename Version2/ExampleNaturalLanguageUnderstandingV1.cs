using IBM.Watson.NaturalLanguageUnderstanding.V1;
using IBM.Watson.NaturalLanguageUnderstanding.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;


namespace Mapchat
{
    public class Category //Create a category class 
    {
        public string label;     //Complete category label
        public string mainlabel; //Main category label
        public double? score;    //Confidence score of the keyword belonging to the Category
        public string keyword;   //The keyword that is extracted to check category

        public Category(string newlabel, double? newscore, string newkw)
        {
            label = newlabel;
            mainlabel = label.Split('/')[1];
            score = newscore;
            keyword = newkw;
        }
        public void printS()
        {
            Debug.Log("                                    " + keyword + "   " + mainlabel + "    " + score);
        }
    }
    public class MainCategory //Class containing only the Main Category and it's score
    {
        public string mainlabel;
        public double? score;

        public MainCategory(string newlabel, double? newscore)
        {
            mainlabel = newlabel;
            score = newscore;
        }
        public void printS()
        {
            Debug.Log("                                    " + mainlabel + "    " + score);
        }

    }
    public class ans //Answer contains the lemma sentence and list of extracted categories per input statement
    {
        public string lemma;
        public List<MainCategory> cat;
        public ans(string newlemma, List<Category> cats)
        {
            cat = new List<MainCategory>();
            lemma = newlemma;
            double? d = 0.00;
            List<string> My_list =  new List<string>();
            List<Category> temp; 

            foreach (var c in cats)
            {
                if (!My_list.Contains(c.mainlabel))
                {
                    My_list.Add(c.mainlabel);
                }
            }
            foreach (var m in My_list)
            {
                temp = cats.FindAll(delegate (Category c) { return c.mainlabel == m; });
                foreach (var t in temp)
                {
                    d = d + t.score;
                }
                d = d / temp.Count;

                cat.Add(new MainCategory(m, d));
            }

        }
        public void prints()
        {
            Debug.Log("                      lemmas: " + lemma);
            foreach(var c in cat)
            {
                c.printS();
            }
        }
    }
    //API key and version date has been ommitted to protect data
    public class ExampleNaturalLanguageUnderstandingV1 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey= ""; //Has been ommitted
		[Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/natural-language-understanding/api\"")]
        [SerializeField]
        private string serviceUrl;
        [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
        [SerializeField]
        private string versionDate = ""; //Has been ommitted
        #endregion

        private NaturalLanguageUnderstandingService service;
        private string lemmaText = "";
        public Dictionary<string, ans> sentenceValues = new Dictionary<string, ans>();
        List<Category> outList;
        public bool set = false;


        public void AnalyzeText(string textInput) //Call the Analyze function with the required input
        {
            Runnable.Run(ExampleAnalyze(textInput));
        }
        public bool GetSentUnderstanding()
        {
            if (sentenceValues.Count == 2)
            {
                foreach (KeyValuePair<string, ans> item in sentenceValues)
                {
                    Debug.Log("mummy: " + item.Key);
                    item.Value.prints();
                }
                return false;
            }
            return true;
        }
        public void printUnderstanding()
        {
            foreach (KeyValuePair<string, ans> item in sentenceValues)
            {
                Debug.Log("mummy: " + item.Key);
                item.Value.prints();
            }
        }
        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
            }

            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            while (!authenticator.CanAuthenticate())
            {
                yield return null;
            }

			service = new NaturalLanguageUnderstandingService(versionDate, authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }
        }

        private IEnumerator ExampleAnalyze(string textInput) //Selects the features we require from the NLU Call
        {
            Features features = new Features()
            {
                Keywords = new KeywordsOptions()
                {
                    Limit = 2,
                    Sentiment = true
                },
                Categories = new CategoriesOptions()
                {
                    Limit = 3,
                    Explanation = true
                },
                Syntax = new SyntaxOptions()
                {
                    Sentences = true,
                    Tokens = new SyntaxOptionsTokens()
                    {
                        PartOfSpeech = true,
                        Lemma = true
                    }
                }
            };
            AnalysisResults analyzeResponse = null;

            service.Analyze(
                callback: (DetailedResponse<AnalysisResults> response, IBMError error) =>
                {
                    analyzeResponse = response.Result;
                    outList = new List<Category>();
                    lemmaText = "";

                    foreach (var cat in analyzeResponse.Categories)
                    {
                        outList.Add(new Category(cat.Label, cat.Score, cat.Explanation.RelevantText[0].Text));
                    }
                    foreach (var tok in analyzeResponse.Syntax.Tokens)
                    {
                        lemmaText = lemmaText + tok.Lemma + " ";
                        set = true;
                    }
                    sentenceValues.Add(textInput, new ans(lemmaText, outList));
                },
                features: features,
                text: textInput
            );

            while (analyzeResponse == null)
                yield return null;
        }

        private IEnumerator ExampleListModels()
        {
            ListModelsResults listModelsResponse = null;

            service.ListModels(
                callback: (DetailedResponse<ListModelsResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1", "ListModels result: {0}", response.Response);
                    listModelsResponse = response.Result;
                }
            );

            while (listModelsResponse == null)
                yield return null;
        }

        private IEnumerator ExampleDeleteModel()
        {
            DeleteModelResults deleteModelResponse = null;

            service.DeleteModel(
                callback: (DetailedResponse<DeleteModelResults> response, IBMError error) =>
                {
                    Log.Debug("NaturalLanguageUnderstandingServiceV1", "DeleteModel result: {0}", response.Response);
                    deleteModelResponse = response.Result;
                },
                modelId: "<modelId>" // Enter model Id here
            );

            while (deleteModelResponse == null)
                yield return null;
        }
    }
}
