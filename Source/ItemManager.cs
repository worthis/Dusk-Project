namespace DuskProject.Source;

using DuskProject.Source.Creatures;
using DuskProject.Source.Dialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ItemManager
{
    private static readonly object InstanceLock = new object();
    private static ItemManager instance;

    private Dictionary<string, Item> _items;

    private ItemManager()
    {
        Console.WriteLine("ItemManager created");
    }

    public static ItemManager Instance
    {
        get
        {
            lock (InstanceLock)
            {
                return instance ??= new ItemManager();
            }
        }
    }

    public void Init()
    {
        LoadItems("Data/items.json");

        Console.WriteLine("ItemManager initialized");
    }

    public bool GetItem(string name, out Item item)
    {
        if (_items.TryGetValue(name, out item))
        {
            return true;
        }

        return false;
    }

    private void LoadItems(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine("Error: Unable to load items file {0}", fileName);
            return;
        }

        using (StreamReader streamReader = new(fileName))
        {
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();

            _items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(jsonData, new StringEnumConverter());
        }

        Console.WriteLine("Items loaded from {0}", fileName);
    }
}
