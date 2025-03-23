using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Chatroom_MAUI;

public static class AppState
{
    public static User CurrentUser { get; set; }
    public static string ServerIp { get; set; }
}

public class User
{
    public string Username { get; set; }

    public User(string username)
    {
        Username = username;
    }

    // Contecta el usuario al server enviando un POST request con los datos del usuario
    public async Task Connect()
    {
        try
        {

            // Serializa el objeto User a JSON, y lo prepara para enviar el POST request
            var json = JsonSerializer.Serialize(this);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Crea una instancia de HttpClient, envia el POST request al endpoint "connect" del server con los datos del usuario, verifica el response
            using var client = new HttpClient();
            var response = await client.PostAsync($"https://{AppState.ServerIp}/api/users/connect", data);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to server: {ex.Message}");
        }
    }

    public async Task Disconnect()
    {
        try
        {

            var json = JsonSerializer.Serialize(this);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync($"https://{AppState.ServerIp}/api/users/disconnect", data);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting from server: {ex.Message}");
        }
    }

    // Envia un mensaje al servidor enviando un POST request con los datos del mensaje
    public async Task SendMessage(string content)
    {
        try
        {

            // Instancia un Mensaje con el contenido dado, serializa el mensaje a JSON y lo prepara para enviar el request
            var message = new Message(content);
            var json = JsonSerializer.Serialize(message);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // Crea una instancia de HttpClient, envia el POST request al endpoint "messages" del server con los datos del mensaje, verifica el response
            using var client = new HttpClient();
            var response = await client.PostAsync($"https://{AppState.ServerIp}/api/messages", data);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }
}

public class Message
{
    public string Username { get; set; }
    public string Content { get; set; }
    public DateTime Time { get; set; }

    // Constructor sin parametros para la deserializacion del JSON
    public Message() { }

    public Message(string content)
    {
        Username = AppState.CurrentUser.Username;
        Content = content;
        Time = DateTime.Now;
    }
}