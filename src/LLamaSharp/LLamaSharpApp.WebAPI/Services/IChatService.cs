﻿using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

public interface IChatService
{
    string Send(SendMessage input);
}