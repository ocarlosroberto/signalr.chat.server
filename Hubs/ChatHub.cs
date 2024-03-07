using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Chat.Server.Hubs
{
    public class ChatHub : Hub
    {
        // public async Task SendMessage(string device, string data)
        // {
        //     await Clients.All.SendAsync("ReceiveMessage", device, data);
        //     Console.WriteLine($"ConnectionId: {Context.ConnectionId} - Mensagem: {data}");
        // }

        private static readonly ConcurrentDictionary<string, string> UserConnectionMap = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            string username = Context.GetHttpContext()?.Request.Query["username"] ?? "Anonimo";
            UserConnectionMap.TryAdd(username, Context.ConnectionId);
            Console.WriteLine($"Usuário conectado: {username} - ConnectionId: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = UserConnectionMap.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            UserConnectionMap.TryRemove(username, out _);
            Console.WriteLine($"Usuário desconectado: {username} - ConnectionId: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
        
        public async Task BroadcastMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveBroadcastMessage", Context.ConnectionId, message);
            Console.WriteLine($"Mensagem de {Context.ConnectionId}: {message}");
        }
        
        public async Task EnviarComandoParaperiferico(string nomePeriferico, string comando)
        {
            string connectionId = UserConnectionMap.FirstOrDefault(x => x.Key == nomePeriferico).Value;
            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceberComando", nomePeriferico, comando, Context.ConnectionId);
                Console.WriteLine($"Enviando comando para {nomePeriferico} [{connectionId}]: {comando}");
            }
            else
                await Clients.Caller.SendAsync("ReceberRetorno", $"Periférico (nomePerlferico] não encontrado!");
        }
        
        public async Task EnviarRetornoParaTela(string retorno, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("ReceberRetorno", retorno);
            Console.WriteLine($"Enviando retorno para {connectionId}: fretorno]");
        }
    }
}
