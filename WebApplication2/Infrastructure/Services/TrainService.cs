﻿using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Infrastructure.Services;

public class TrainService
{
    private readonly ApplicationDbContext _context;

    public TrainService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(CreateTrainModelIn trainModel, string schemaPath)
    {
        var train = new Train()
        {
            typeName = trainModel.TypeName,
            schemaPath = schemaPath,
            trainRows = trainModel.TrainRows,
            trainSeats = trainModel.TrainSeats
        };
        _context.Trains.Add(train);
        await _context.SaveChangesAsync();
    }
}
