namespace DuskProject.Source;

using DuskProject.Source.Dialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ItemManager
{
    private static readonly object InstanceLock = new object();
    private static ItemManager instance;

    private Dictionary<string, Item> _items = new();

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

            var items = JsonConvert.DeserializeObject<List<Item>>(jsonData, new StringEnumConverter());

            foreach (var item in items)
            {
                _items.Add(item.Id, item);
            }
        }

        Console.WriteLine("Items loaded from {0}", fileName);
    }
}
