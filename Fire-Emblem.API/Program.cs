using Fire_Emblem.API.Business.Context.Abilities;
using Fire_Emblem.API.Business.Context.Characters;
using Fire_Emblem.API.Business.Context.Equips;
using Fire_Emblem.API.Business.Context.PersonalAbilities;
using Fire_Emblem.API.Business.Context.UnitClasses;
using Fire_Emblem.API.Business.Repository.Abilities;
using Fire_Emblem.API.Business.Repository.Characters;
using Fire_Emblem.API.Business.Repository.Equips;
using Fire_Emblem.API.Business.Repository.PersonalAbilities;
using Fire_Emblem.API.Business.Repository.UnitClasses;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAbilitiesContext, AbilitiesContext>();
builder.Services.AddSingleton<IAbilitiesRepository, AbilitiesRepository>();
builder.Services.AddSingleton<IUnitClassesContext, UnitClassesContext>();
builder.Services.AddSingleton<IUnitClassesRepository, UnitClassesRepository>();
builder.Services.AddSingleton<IPersonalAbilitiesContext, PersonalAbilitiesContext>();
builder.Services.AddSingleton<IPersonalAbilitiesRepository, PersonalAbilitiesRepository>();
builder.Services.AddSingleton<IEquipmentContext, EquipmentContext>();
builder.Services.AddSingleton<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddSingleton<ICharactersContext, CharactersContext>();
builder.Services.AddSingleton<ICharactersRepository, CharactersRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
