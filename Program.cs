using System.Threading.Tasks;

namespace BotTemplate {
    class Program {
        static async Task Main(string[] args) {
            await new Bot().RunAsync();
        }
    }
}
