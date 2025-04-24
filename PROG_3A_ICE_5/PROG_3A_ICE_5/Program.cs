namespace PROG_3A_ICE_5
{

    //Code attribution for the below:
    //Author: Open AI
    //AI Model used: Chat GPT
    //Chat Link: https://chatgpt.com/share/6809db04-ec4c-8002-a76f-f1afc8f8af77
    //Date Accessed: 24 April 2025

    class Program
    {
        static void Main(string[] args)
        {
            var configurator = new VehicleConfigurator();

            while (true)
            {
                Console.Clear();
                WriteHeader("🚗 Welcome to Vehicle Factory Inc. 🚗");
                Console.WriteLine("1. Order a Vehicle");
                Console.WriteLine("2. Exit");
                Console.Write("\nPlease enter choice [1–2]: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        PlaceOrder(configurator);
                        break;
                    case "2":
                        return;
                    default:
                        WriteError("Invalid choice. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void PlaceOrder(VehicleConfigurator configurator)
        {
            // 1) Vehicle selection
            Console.Clear();
            WriteHeader("🔧 Vehicle Selection");
            var vehicleType = SelectFromMenu("Select vehicle type:", configurator.AvailableVehicleTypes);

            // 2) Engine selection
            Console.Clear();
            WriteHeader($"🔌 Engine Selection for {vehicleType}");
            var engineFactory = SelectFromMenu(
                "Select engine type:",
                configurator.GetSupportedEngineOptions(vehicleType)
            );

            // 3) Create & configure
            var vehicle = configurator.CreateVehicleWithEngine(vehicleType, engineFactory);

            // 4) Summary
            Console.Clear();
            WriteHeader("🎉 Order Summary");
            Console.WriteLine($"Vehicle:  {vehicle.GetType().Name}");
            Console.WriteLine($"Engine:   {vehicle.Engine.GetType().Name}");
            Console.WriteLine($"Details:  {vehicle.Describe()}\n");
            Console.WriteLine("Thank you for your order! Press any key to return to main menu...");
            Console.ReadKey();
        }

        // Generic menu helper: show a list, return chosen item
        static T SelectFromMenu<T>(string prompt, IReadOnlyList<T> options)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                for (int i = 0; i < options.Count; i++)
                    Console.WriteLine($"{i + 1}. {options[i]}");
                Console.Write($"\nEnter choice [1–{options.Count}]: ");

                if (int.TryParse(Console.ReadLine(), out int idx)
                    && idx >= 1 && idx <= options.Count)
                {
                    return options[idx - 1];
                }

                WriteError("Invalid selection, please try again.\n");
            }
        }

        static void WriteHeader(string text)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.WriteLine(new string('─', text.Length));
            Console.ForegroundColor = prev;
        }

        static void WriteError(string message)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = prev;
        }
    }

    // ---------------------------------------------------
    // Encapsulates Factory Method + Abstract Factory
    // ---------------------------------------------------
    public class VehicleConfigurator
    {
        // Available vehicles
        public IReadOnlyList<VehicleType> AvailableVehicleTypes =>
            new List<VehicleType> { VehicleType.Car, VehicleType.Motorcycle, VehicleType.Truck };

        // Filters out unsupported engine choices (e.g. no Hybrid on Motorcycle)
        public IReadOnlyList<IEngineFactory> GetSupportedEngineOptions(VehicleType vehicleType)
        {
            var list = new List<IEngineFactory>
            {
                new ElectricEngineFactory(),
                new GasolineEngineFactory()
            };

            if (vehicleType != VehicleType.Motorcycle)
                list.Add(new HybridEngineFactory());

            return list;
        }

        // Applies both patterns
        public IVehicle CreateVehicleWithEngine(VehicleType type, IEngineFactory engineFactory)
        {
            var vehicle = VehicleFactory.Create(type);
            vehicle.Engine = engineFactory.CreateEngine();
            return vehicle;
        }
    }

    // ---------------------------------------------------
    // Factory Method: Vehicle creation
    // ---------------------------------------------------
    public static class VehicleFactory
    {
        public static IVehicle Create(VehicleType type) => type switch
        {
            VehicleType.Car => new Car(),
            VehicleType.Motorcycle => new Motorcycle(),
            VehicleType.Truck => new Truck(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public enum VehicleType
    {
        Car,
        Motorcycle,
        Truck
    }

    public interface IVehicle
    {
        IEngine Engine { get; set; }
        string Describe();
    }

    public class Car : IVehicle
    {
        public IEngine Engine { get; set; }
        public string Describe() =>
            $"A comfortable Car powered by {Engine.Type}.";
    }

    public class Motorcycle : IVehicle
    {
        public IEngine Engine { get; set; }
        public string Describe() =>
            $"A nimble Motorcycle with a {Engine.Type} engine.";
    }

    public class Truck : IVehicle
    {
        public IEngine Engine { get; set; }
        public string Describe() =>
            $"A heavy-duty Truck running on {Engine.Type}.";
    }

    // ---------------------------------------------------
    // Abstract Factory: Engine creation
    // ---------------------------------------------------
    public interface IEngine
    {
        string Type { get; }
    }

    public interface IEngineFactory
    {
        IEngine CreateEngine();
    }

    public class ElectricEngineFactory : IEngineFactory
    {
        public IEngine CreateEngine() => new ElectricEngine();
        public override string ToString() => "Electric";
    }

    public class GasolineEngineFactory : IEngineFactory
    {
        public IEngine CreateEngine() => new GasolineEngine();
        public override string ToString() => "Gasoline";
    }

    public class HybridEngineFactory : IEngineFactory
    {
        public IEngine CreateEngine() => new HybridEngine();
        public override string ToString() => "Hybrid";
    }

    public class ElectricEngine : IEngine
    {
        public string Type => "Electric";
    }

    public class GasolineEngine : IEngine
    {
        public string Type => "Gasoline";
    }

    public class HybridEngine : IEngine
    {
        public string Type => "Hybrid";
    }
}

