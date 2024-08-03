namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Combat;
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class CombatManager
    {
        private static CombatManager instance;
        private static object instanceLock = new object();

        private Dictionary<string, EnemyBase> _enemies = new Dictionary<string, EnemyBase>();

        private EnemyBase _enemy;
        private CombatPhase _phase = CombatPhase.Intro;
        private int _timer = 0;

        private CombatManager()
        {
        }

        public static CombatManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CombatManager();
                        Console.WriteLine("CombatManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            LoadEnemies("Data/enemies.json");

            Console.WriteLine("CombatManager initialized");
        }

        public void LoadEnemies(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load enemies file {0}", fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _enemies = JsonConvert.DeserializeObject<Dictionary<string, EnemyBase>>(jsonData, new StringEnumConverter());
            }

            Console.WriteLine("Enemies loaded from {0}", fileName);
        }

        public void Update()
        {
            UpdateIntro();
        }

        public void Render()
        {
        }

        private void UpdateIntro()
        {
            if (!_phase.Equals(CombatPhase.Intro))
            {
                return;
            }

            _timer--;

            // enemy.renderoffset = 0 - _timer * 10;
            if (_timer < 0)
            {
                _phase = CombatPhase.Input;
            }
        }
    }
}
