//רק בסיעתא דשמיא
using System.Text;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add database context
builder.Services.AddDbContext<ToDoDbContext>();
//swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());   

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/items",  (ToDoDbContext db) =>{ return   db.Items.ToList();});
   

app.MapPost("/items", async (Item item, ToDoDbContext toDoDbContext) =>

{
    Item newItem = new Item
    {
        IsComplete = item.IsComplete,
        Name = item.Name
    };
    toDoDbContext.Items.Add(newItem);
    await toDoDbContext.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});


app.MapPut("/items/{id}", async (ToDoDbContext context, int id) =>
{
    var exsistItem = await context.Items.FindAsync(id);
    if (exsistItem is null) return Results.NotFound();

    //exsistItem.Name = item.Name;
    exsistItem.IsComplete = true;

    await context.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/items/{id}", async (ToDoDbContext context, int id) =>
{
    var exsistItem = await context.Items.FindAsync(id);
    if (exsistItem is null) return Results.NotFound();

    context.Items.Remove(exsistItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
