using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Windows;

namespace Test_Chat_GPT3
{
    /// <summary>
    /// Clase que define la información serializable de la aplicación
    /// Autor: Alejandro Olivo 07-2022
    /// </summary>
    public class DataXml
    {
        #region ATTRIBUTES

        // Path donde se encuentra el XML
        private string _configPathXML;
        private string _promptPathJSON;

        ConfigData _configData = null;
        Prompt _prompt = null;

        #endregion

        #region PROPERTIES
        public string ConfigPathXML
        {
            get { return _configPathXML; }
            set { _configPathXML = value; }
        }
        public string PromptPathXML
        {
            get { return _promptPathJSON; }
            set { _promptPathJSON = value; }

        }
        public ConfigData ConfigDataP
        {
            get { return _configData; }
            set { _configData = value; }
        }
        

        public Prompt PromptP
        {
            get { return _prompt; }
            set { _prompt = value; }
        }

        #endregion

        #region CONSTRUCTOR

        public DataXml(string ConfigPath)
        {
            //Ruta del archivo
            _configPathXML = ConfigPath;

            //Cargamos configuración
            LoadConfigData(_configPathXML);
        }

        public DataXml(string ConfigPath, string PromptPath, string StylePath)
        {
            //Ruta del archivo
            _configPathXML = ConfigPath;
            _promptPathJSON = PromptPath;

            //Cargamos configuración
            LoadConfigData(_configPathXML);
            LoadPrompt(_promptPathJSON);
            
        }
        
        public DataXml()
        {

        }

        #endregion

        #region METHODS 

        /// <summary>
        /// Función para comprobar que existe un archivo y lanzar una excepción si no
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        internal void FileExists(string filePath)
        {
            string message = string.Empty;

            if (System.IO.File.Exists(filePath) == false)
            {
                message = "File : " + filePath + " not found";
                throw new Exception(message);
            }
        }

        

        /// <summary>
        /// Método para cargar la info de configuración
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfigData(string path)
        {
            try
            {
                //Guardamos el path
                _configPathXML = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_configPathXML))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _configData = SerializationManager.ReadConfigData(_configPathXML);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        /// <summary>
        /// Método para cargar la info de los tags
        /// </summary>
        /// <param name="path"></param>
        public void LoadPrompt(string path)
        {
            try
            {
                //Guardamos el path
                _promptPathJSON = path; // System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Properties.XmlPathData.Default.NameDirectoryPath);

                //Comprobamos el directorio
                if (!System.IO.File.Exists(_promptPathJSON))
                    throw new Exception("File not found");

                //Importamos la info desde el archivo
                _prompt = SerializationManager.ReadPrompt(_promptPathJSON);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in data loading: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportConfigData(string nameDir)
        {

            if (!nameDir.Contains(".xml"))
                nameDir += ".xml";

            if (_configData != null)
                SerializationManager.WriteConfigData(nameDir, _configData);
        }
        

        /// <summary>
        /// Export data from serializable classes to XML files
        /// </summary>
        /// <param name="nameDir"></param>
        public void ExportPrompt(string nameDir)
        {

            if (!nameDir.Contains(".json"))
                nameDir += ".json";

            if (_prompt != null)
                SerializationManager.WritePrompt(nameDir, _prompt);
        }

        #endregion

        #region Clases Serializables

        /// <summary>
        /// Clase serializable que define los parámetros de configuración de la app
        /// </summary>
        [Serializable()]
        [XmlRoot("Config", Namespace = "", IsNullable = false)]
        public class ConfigData
        {
            #region Properties
            public string empresa { get; set; }
            public string idioma { get; set; }
            public string appVersion { get; set; }
            public string newVersion { get; set; }
            public string isSurfaceOrPC { get; set; }
            public string lastUserLogged { get; set; }
            public ObservableCollection<User> users { get; set; }

            public string serverContentMainPath = "";

            #endregion

        }

        /// <summary>
        /// Clase serializable que define los tags de la app
        /// </summary>
        [Serializable()]
        [XmlRoot("Prompt", Namespace = "", IsNullable = false)]
        public class Prompt
        {
            #region Properties

            public string StartText;

            #endregion


        }
        

        public class User : IEquatable<User>
        {
            #region Properties
            public string Name { get; set; }
            public string Pwd { get; set; }
            #endregion

            #region Constructor
            public User(string Name, string pwd)
            {
                this.Name = Name;
                this.Pwd = pwd;
            }
            public User()
            { }
            #endregion

            public bool Equals(User other)
            {
                return other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }


        #endregion

    }
    
    

        /// <summary>
        /// Clase para hacer la serialización y deserialización en XML o JSON
        /// Autor: Alejandro Olivo 07-2022
        /// </summary>
        public class SerializationManager
        {

            #region CONSTRUCTOR

            public SerializationManager()
            {

            }

            #endregion

            #region READ METHODS

            public static DataXml.ConfigData ReadConfigData(string file_combined)
            {
                //Objeto de la clase Folder
                DataXml.ConfigData confConfig = new DataXml.ConfigData();

                //Stream reader
                using (StreamReader sr = new StreamReader(file_combined))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DataXml.ConfigData));

                    confConfig = (DataXml.ConfigData)serializer.Deserialize(sr);

                    sr.Close();
                }

                return confConfig;
            }
        

            public static DataXml.Prompt ReadPrompt(string file_combined)
            {

                string pth = file_combined.Replace(".xml", ".json");

                // read file into a string and deserialize JSON to a type
                //DataXml.Prompt tags = JsonConvert.DeserializeObject<DataXml.Prompt>(File.ReadAllText(file_combined.Replace(".xml",".json")));

                DataXml.Prompt text;

                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(pth))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    text = (DataXml.Prompt)serializer.Deserialize(file, typeof(DataXml.Prompt));
                }


                return text;
            }

            #endregion

            #region WRITE METHODS

            public static void WriteConfigData(string file_combined, DataXml.ConfigData config_Data)
            {
                using (StreamWriter sr = new StreamWriter(file_combined))
                {
                    XmlSerializer xSerializer = new XmlSerializer(typeof(DataXml.ConfigData));

                    DataXml.ConfigData configData = new DataXml.ConfigData();
                    configData = config_Data;

                    xSerializer.Serialize(sr, (DataXml.ConfigData)configData);
                    sr.Close();
                }
            }


            public static void WritePrompt(string file_combined, DataXml.Prompt tags_Data)
            {
                string pth = "";

                if (!file_combined.Contains(".json"))
                    if (file_combined.Contains(".xml"))
                        pth = file_combined.Replace(".xml", ".json");
                    else
                        pth = file_combined + ".json";
                else
                    pth = file_combined;


                var json = JsonConvert.SerializeObject(tags_Data, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                using (StreamWriter file = File.CreateText(pth))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, tags_Data);
                }

            }


            #endregion

        }
    


}
