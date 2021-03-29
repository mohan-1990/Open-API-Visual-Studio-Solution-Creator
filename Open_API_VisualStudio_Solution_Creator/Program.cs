using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BSG_API_Solution_BoilerPlate_Tool
{
    class Program
    {
        public static string pwd = System.IO.Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello and Welcome!");
            Console.WriteLine("This tool can create an Open API Visual Studio Solution for you.");
            Console.WriteLine("Please answer the following few questions.");

            string apiBaseURL;
            string apiName;

            int numberOfControllers;
            string numberOfControllersTemp;
            string[] controllerNames;

            int numberOfModels;
            string numberOfModelsTemp;
            string[] modelNames;

            do
            {
                Console.WriteLine("API Name? For example, Service Management");
                apiName = Console.ReadLine();
            } while (string.IsNullOrEmpty(apiName));

            do
            {
                Console.WriteLine("Base URL of the API? \nNotes:- \n1) Need to start with api/ \n2) Ends without trailing / \nValid Example:- \napi/services/v1 \nInvalid Example \n/api/cahce/v1/");
                apiBaseURL = Console.ReadLine();
            } while (!apiBaseURL.StartsWith("api/") || apiBaseURL.EndsWith("/"));            

            do
            {
                Console.WriteLine("How many controllers the API needs? For example 2. Minimum 1.");
                numberOfControllersTemp = Console.ReadLine();
            } while (!int.TryParse(numberOfControllersTemp, out numberOfControllers) && numberOfControllers < 1);

            controllerNames = new string[numberOfControllers];

            for (int i = 1; i <= numberOfControllers; ++i)
            {
                string msgToPrint;
                string controllerName;
                if (i == 1)
                {
                    msgToPrint = "Name of controller " + i + "? For example Service";
                }
                else
                {
                    msgToPrint = "Name of controller " + i + "?";
                }

                do
                {
                    Console.WriteLine(msgToPrint);
                    controllerName = Console.ReadLine();
                } while (string.IsNullOrEmpty(controllerName));

                controllerNames[i - 1] = controllerName;
            }

            do
            {
                Console.WriteLine("How many database models the API needs? For example 3. Enter 0 if you want to generate database models later.");
                numberOfModelsTemp = Console.ReadLine();
            } while (!int.TryParse(numberOfModelsTemp, out numberOfModels));

            modelNames = new string[numberOfModels];

            for (int i = 1; i <= numberOfModels; ++i)
            {
                string msgToPrint;
                string modelName;
                if (i == 1)
                {
                    msgToPrint = "Name of database model " + i + "? For example Service";
                }
                else
                {
                    msgToPrint = "Name of database model " + i + "?";
                }

                do
                {
                    Console.WriteLine(msgToPrint);
                    modelName = Console.ReadLine();
                } while (string.IsNullOrEmpty(modelName));

                modelNames[i - 1] = modelName;
            }


            // Prepare the required template variables

            // @SolutionName@
            string solutionName = apiName;
            // @SolutionName_UnderscoreSeperated@
            string solutionNameUnderscoreSeperated = solutionName.Replace(" ", "_");
            // @SolutionName_WhiteSpaceRemoved@
            string solutionNameWhiteSpaceRemoved = solutionName.Replace(" ", "");
            // @TCPPort1@
            Random random1 = new Random();
            int tcpPort1 = random1.Next(51500, 51999);
            // @TCPPort2@
            Random random2 = new Random();
            int tcpPort2 = random2.Next(44300, 44399);

            // Mapping
            Dictionary<string, object> templateVariableToValueMap = new Dictionary<string, object>();

            templateVariableToValueMap.Add("@SolutionName@", solutionName);
            templateVariableToValueMap.Add("@SolutionName_UnderscoreSeperated@", solutionNameUnderscoreSeperated);
            templateVariableToValueMap.Add("@SolutionName_WhiteSpaceRemoved@", solutionNameWhiteSpaceRemoved);

            templateVariableToValueMap.Add("@ProjectName@", solutionName);
            templateVariableToValueMap.Add("@ProjectName_UnderscoreSeperated@", solutionNameUnderscoreSeperated);
            templateVariableToValueMap.Add("@ProjectName_WhiteSpaceRemoved@", solutionNameWhiteSpaceRemoved);

            templateVariableToValueMap.Add("@TCPPort1@", tcpPort1);
            templateVariableToValueMap.Add("@TCPPort2@", tcpPort2);

            templateVariableToValueMap["@BaseURL@"] = apiBaseURL;

            // Recepie
            // Step 1 Create Base solution
            Console.WriteLine("Step 1: Creating Visual Studio solution " + solutionName + ".sln");
            BaseSolution(templateVariableToValueMap);

            // Step 2 Create Base project folders and files
            Console.WriteLine("Step 2: Creating .NET Core Web API project " + solutionName + ".csproj");
            BaseProject(templateVariableToValueMap);

            // Step 3 Create Base project properties folders and files
            Console.WriteLine("Step 3: Creating " + solutionName + " properties folders and files");
            ProjectProperties(templateVariableToValueMap);

            // Step 4 Create Controllers            
            templateVariableToValueMap.Add("@ControllerNames@", controllerNames);
            Console.WriteLine("Step 4: Creating controllers");
            Controllers(templateVariableToValueMap);            

            if (numberOfModels > 0)
            // Step 5 Create Models
            {
                templateVariableToValueMap.Add("@ModelNames@", modelNames);
                templateVariableToValueMap.Add("@ModelReference@", "public DbSet<@ModelName@> @ModelName@s { get; set; }");
                Console.WriteLine("Step 5: Creating database models");
                Models(templateVariableToValueMap);
                DataBase(templateVariableToValueMap);
            }

            // Step 6 Create Filters
            Filters(templateVariableToValueMap);
        }

        static void BaseSolution(Dictionary<string, object> templateVariableToValueMap)
        {
            
            string solutionDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString());
            Directory.CreateDirectory(solutionDirectory);

            string solutionFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.sln.template"));

            solutionFileTemplate = solutionFileTemplate.Replace("@SolutionName@", templateVariableToValueMap["@SolutionName@"].ToString());
            solutionFileTemplate = solutionFileTemplate.Replace("@ProjectName@", templateVariableToValueMap["@ProjectName@"].ToString());
            solutionFileTemplate = solutionFileTemplate.Replace("@Guid1@", Guid.NewGuid().ToString());
            solutionFileTemplate = solutionFileTemplate.Replace("@Guid2@", Guid.NewGuid().ToString());
            solutionFileTemplate = solutionFileTemplate.Replace("@Guid3@", Guid.NewGuid().ToString());

            StreamWriter solutionFileWriter = new StreamWriter(File.Open(Path.Combine(solutionDirectory, templateVariableToValueMap["@SolutionName@"].ToString() + ".sln"), FileMode.OpenOrCreate));
            solutionFileWriter.Write(solutionFileTemplate);

            #if DEBUG
                Console.WriteLine("Writing the following content to " + templateVariableToValueMap["@SolutionName@"].ToString() + ".sln");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(solutionFileTemplate);
            #endif

            solutionFileWriter.Flush();
            solutionFileWriter.Close();

        }


        static void BaseProject(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            Directory.CreateDirectory(projectDirectory);

            // Create ProjectName.csproj file
            string projectFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.@ProjectName@.csproj.template"));
            projectFileTemplate = projectFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());
            StreamWriter projectFileWriter = new StreamWriter(File.Open(Path.Combine(projectDirectory, templateVariableToValueMap["@ProjectName@"].ToString() + ".csproj"), FileMode.OpenOrCreate));
            projectFileWriter.Write(projectFileTemplate);
            #if DEBUG
                Console.WriteLine("Writing the following content to " + templateVariableToValueMap["@SolutionName@"].ToString() + "/" + templateVariableToValueMap["@ProjectName@"].ToString() + ".csproj");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(projectFileTemplate);
            #endif
            projectFileWriter.Flush();
            projectFileWriter.Close();

            // Create ProjectName/Program.cs file
            string programFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Program.cs.template"));
            programFileTemplate = programFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());
            StreamWriter programFileWriter = new StreamWriter(File.Open(Path.Combine(projectDirectory, "Program.cs"), FileMode.OpenOrCreate));
            programFileWriter.Write(programFileTemplate);
            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/Program.cs");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(programFileTemplate);
            #endif
            programFileWriter.Flush();
            programFileWriter.Close();

            // Create ProjectName/Startup.cs file
            string startupFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Startup.cs.template"));
            startupFileTemplate = startupFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());
            startupFileTemplate = startupFileTemplate.Replace("@SolutionName_WhiteSpaceRemoved@", templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString());
            startupFileTemplate = startupFileTemplate.Replace("@SolutionName@", templateVariableToValueMap["@SolutionName@"].ToString());
            StreamWriter startupFileWriter = new StreamWriter(File.Open(Path.Combine(projectDirectory, "Startup.cs"), FileMode.OpenOrCreate));
            startupFileWriter.Write(startupFileTemplate);
            #if DEBUG
                Console.WriteLine("Writing the following content to " + templateVariableToValueMap["@SolutionName@"].ToString() + "/Startup.cs");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(startupFileTemplate);
            #endif
            startupFileWriter.Flush();
            startupFileWriter.Close();

            // Create ProjectName/appSettings.json file
            string appSettingsFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.appSettings.json.template"));
            appSettingsFileTemplate = appSettingsFileTemplate.Replace("@SolutionName_WhiteSpaceRemoved@", templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString());
            appSettingsFileTemplate = appSettingsFileTemplate.Replace("@SolutionName@", templateVariableToValueMap["@SolutionName@"].ToString());
            appSettingsFileTemplate = appSettingsFileTemplate.Replace("@BaseURL@", templateVariableToValueMap["@BaseURL@"].ToString());
            StreamWriter appSettingsFileWriter = new StreamWriter(File.Open(Path.Combine(projectDirectory, "appSettings.json"), FileMode.OpenOrCreate));
            appSettingsFileWriter.Write(appSettingsFileTemplate);
            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/appSettings.json");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(appSettingsFileTemplate);
            #endif
            appSettingsFileWriter.Flush();
            appSettingsFileWriter.Close();

            // Create ProjectName/appSettings.Development.json file
            string appSettingsDevelopmentFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.appSettings.Development.json.template"));            
            StreamWriter appSettingsDevelopmentFileWriter = new StreamWriter(File.Open(Path.Combine(projectDirectory, "appSettings.Development.json"), FileMode.OpenOrCreate));
            appSettingsDevelopmentFileWriter.Write(appSettingsDevelopmentFileTemplate);
            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/appSettings.Development.json");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(appSettingsDevelopmentFileTemplate);
            #endif
            appSettingsDevelopmentFileWriter.Flush();
            appSettingsDevelopmentFileWriter.Close();
        }

        static void ProjectProperties(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            string propertiesDirectory = Path.Combine(projectDirectory, "Properties");
            Directory.CreateDirectory(propertiesDirectory);

            string launchSettingsFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Properties.launchSettings.json.template"));

            launchSettingsFileTemplate = launchSettingsFileTemplate.Replace("@SolutionName_UnderscoreSeperated@", templateVariableToValueMap["@SolutionName_UnderscoreSeperated@"].ToString());
            launchSettingsFileTemplate = launchSettingsFileTemplate.Replace("@BaseURL@", templateVariableToValueMap["@BaseURL@"].ToString());
            launchSettingsFileTemplate = launchSettingsFileTemplate.Replace("@TCPPort1@", templateVariableToValueMap["@TCPPort1@"].ToString());
            launchSettingsFileTemplate = launchSettingsFileTemplate.Replace("@TCPPort2@", templateVariableToValueMap["@TCPPort2@"].ToString());

            StreamWriter launchSettingsFileWriter = new StreamWriter(File.Open(Path.Combine(propertiesDirectory, "launchSettings.json"), FileMode.OpenOrCreate));
            launchSettingsFileWriter.Write(launchSettingsFileTemplate);

            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/Properties/launchSettings.json");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(launchSettingsFileTemplate);
            #endif

            launchSettingsFileWriter.Flush();
            launchSettingsFileWriter.Close();
        }

        static void Controllers(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            string controllersDirectory = Path.Combine(projectDirectory, "Controllers");
            Directory.CreateDirectory(controllersDirectory);

            string[] controllerNames = (string[]) templateVariableToValueMap["@ControllerNames@"];

            foreach(string controllerName in controllerNames)
            {
                string controllerFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Controllers.@ControllerName@.cs.template"));

                controllerFileTemplate = controllerFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());
                controllerFileTemplate = controllerFileTemplate.Replace("@SolutionName_WhiteSpaceRemoved@", templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString());
                controllerFileTemplate = controllerFileTemplate.Replace("@BaseURL@", templateVariableToValueMap["@BaseURL@"].ToString());
                controllerFileTemplate = controllerFileTemplate.Replace("@ControllerName@", controllerName);
                controllerFileTemplate = controllerFileTemplate.Replace("@SolutionName_WhiteSpaceRemoved_FirstLetterLowerCase@", ToLowerFirstChar(templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString()));

                StreamWriter controllerFileWriter = new StreamWriter(File.Open(Path.Combine(controllersDirectory, controllerName + "Controller.cs"), FileMode.OpenOrCreate));
                controllerFileWriter.Write(controllerFileTemplate);

                #if DEBUG
                    Console.WriteLine("Writing the following content to " + projectDirectory + "/Controllers/" + controllerName + "Controller.cs");
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(controllerFileTemplate);
                #endif

                controllerFileWriter.Flush();
                controllerFileWriter.Close();
            }
        }

        static void Models(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            string modelsDirectory = Path.Combine(projectDirectory, "Models");
            Directory.CreateDirectory(modelsDirectory);

            string[] modelNames = (string[])templateVariableToValueMap["@ModelNames@"];

            foreach (string modelName in modelNames)
            {
                string modelFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Models.@ModelName@.cs.template"));                

                modelFileTemplate = modelFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());                
                modelFileTemplate = modelFileTemplate.Replace("@ModelName@", modelName);                

                StreamWriter modelFileWriter = new StreamWriter(File.Open(Path.Combine(modelsDirectory, modelName + ".cs"), FileMode.OpenOrCreate));
                modelFileWriter.Write(modelFileTemplate);

                #if DEBUG
                    Console.WriteLine("Writing the following content to " + projectDirectory + "/Models/" + modelName + ".cs");
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(modelFileTemplate);
                #endif

                modelFileWriter.Flush();
                modelFileWriter.Close();
            }
        }

        static void DataBase(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            string databaseDirectory = Path.Combine(projectDirectory, "Database");
            Directory.CreateDirectory(databaseDirectory);
            
                string databaseFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Database.@SolutionName_WhiteSpaceRemoved@Context.cs.template"));

                databaseFileTemplate = databaseFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());
                databaseFileTemplate = databaseFileTemplate.Replace("@SolutionName_WhiteSpaceRemoved@", templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString());

            string[] modelNames = (string[])templateVariableToValueMap["@ModelNames@"];
            StringBuilder modelReferences = new StringBuilder();

            foreach (string modelName in modelNames)
            {
                string modelReference = templateVariableToValueMap["@ModelReference@"].ToString().Replace("@ModelName@", modelName);
                modelReferences.Append(modelReference + "\n");
            }
            databaseFileTemplate = databaseFileTemplate.Replace("@ModelReferences@", modelReferences.ToString());

            StreamWriter databaseFileWriter = new StreamWriter(File.Open(Path.Combine(databaseDirectory, templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString() + "Context.cs"), FileMode.OpenOrCreate));
            databaseFileWriter.Write(databaseFileTemplate);

            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/Database/" + templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString() + "Context.cs");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(databaseFileTemplate);
            #endif

            databaseFileWriter.Flush();
            databaseFileWriter.Close();            
        }

        static void Filters(Dictionary<string, object> templateVariableToValueMap)
        {
            string projectDirectory = Path.Combine(pwd, templateVariableToValueMap["@SolutionName@"].ToString(), templateVariableToValueMap["@ProjectName@"].ToString());
            string filtersDirectory = Path.Combine(projectDirectory, "Filters");
            Directory.CreateDirectory(filtersDirectory);

            string filterFileTemplate = File.ReadAllText(Path.Combine(pwd, "Templates", "@SolutionName@.Filters.AddAuthHeaderOperationFilter.cs.template"));

            filterFileTemplate = filterFileTemplate.Replace("@ProjectName_UnderscoreSeperated@", templateVariableToValueMap["@ProjectName_UnderscoreSeperated@"].ToString());

            StreamWriter filterFileWriter = new StreamWriter(File.Open(Path.Combine(filtersDirectory, "AddAuthHeaderOperationFilter.cs"), FileMode.OpenOrCreate));
            filterFileWriter.Write(filterFileTemplate);

            #if DEBUG
                Console.WriteLine("Writing the following content to " + projectDirectory + "/Filters/" + templateVariableToValueMap["@SolutionName_WhiteSpaceRemoved@"].ToString() + "AddAuthHeaderOperationFilter.cs");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(filterFileTemplate);
            #endif

            filterFileWriter.Flush();
            filterFileWriter.Close();
        }

        static string ToLowerFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
