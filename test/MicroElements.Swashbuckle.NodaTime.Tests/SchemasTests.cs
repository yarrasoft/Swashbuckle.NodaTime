using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using NodaTime.Serialization.SystemTextJson;
using Xunit;

namespace MicroElements.Swashbuckle.NodaTime.Tests
{
    public class SchemasTests
    {
        [Fact]
        public void NewtonsoftJsonSettingsTest()
        {
            IDateTimeZoneProvider dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
            DateTime dateTimeUtc = new DateTime(2020, 05, 23, 10, 30, 50, DateTimeKind.Utc);

            var schemaExamples = new SchemaExamples(dateTimeZoneProvider, dateTimeUtc, "Europe/Moscow");
            var nodaTimeSchemaSettings = new JsonSerializerSettings()
                .ConfigureForNodaTime(dateTimeZoneProvider)
                .CreateNodaTimeSchemaSettingsForNewtonsoftJson(schemaExamples: schemaExamples);

            CheckGeneratedSchema(nodaTimeSchemaSettings);
        }

        [Fact]
        public void SystemTextJsonSettingsTest()
        {
            IDateTimeZoneProvider dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
            DateTime dateTimeUtc = new DateTime(2020, 05, 23, 10, 30, 50, DateTimeKind.Utc);

            var schemaExamples = new SchemaExamples(dateTimeZoneProvider, dateTimeUtc, "Europe/Moscow");
            var jsonSerializerOptions = new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};
            var nodaTimeSchemaSettings = jsonSerializerOptions
                .ConfigureForNodaTime(dateTimeZoneProvider)
                .CreateNodaTimeSchemaSettingsForSystemTextJson(schemaExamples: schemaExamples);

            CheckGeneratedSchema(nodaTimeSchemaSettings);
        }

        private static void CheckGeneratedSchema(NodaTimeSchemaSettings nodaTimeSchemaSettings)
        {
            Schemas schemas = new SchemasFactory(nodaTimeSchemaSettings).CreateSchemas();

            schemas.Instant().Type.Should().Be(JsonSchemaType.String);
            schemas.Instant().Format.Should().Be("date-time");
            schemas.Instant().Example.AsString().Should().Be("2020-05-23T10:30:50Z");

            schemas.LocalDate().Type.Should().Be(JsonSchemaType.String);
            schemas.LocalDate().Format.Should().Be("date");
            schemas.LocalDate().Example.AsString().Should().Be("2020-05-23");

            schemas.LocalTime().Type.Should().Be(JsonSchemaType.String);
            schemas.LocalTime().Format.Should().Be(null);
            schemas.LocalTime().Example.AsString().Should().Be("13:30:50");

            schemas.LocalDateTime().Type.Should().Be(JsonSchemaType.String);
            schemas.LocalDateTime().Format.Should().Be(null);
            schemas.LocalDateTime().Example.AsString().Should().Be("2020-05-23T13:30:50");

            schemas.OffsetDateTime().Type.Should().Be(JsonSchemaType.String);
            schemas.OffsetDateTime().Format.Should().Be("date-time");
            schemas.OffsetDateTime().Example.AsString().Should().Be("2020-05-23T13:30:50+03:00");

            schemas.ZonedDateTime().Type.Should().Be(JsonSchemaType.String);
            schemas.ZonedDateTime().Format.Should().Be(null);
            schemas.ZonedDateTime().Example.AsString().Should().Be("2020-05-23T13:30:50+03 Europe/Moscow");

            schemas.Interval().Type.Should().Be(JsonSchemaType.Object);
            schemas.Interval().Properties["Start"].Example.AsString().Should().Be("2020-05-23T10:30:50Z");
            schemas.Interval().Properties["End"].Example.AsString().Should().Be("2020-05-24T11:31:51.001Z");

            schemas.DateInterval().Type.Should().Be(JsonSchemaType.Object);
            schemas.DateInterval().Properties["Start"].Example.AsString().Should().Be("2020-05-23");
            schemas.DateInterval().Properties["End"].Example.AsString().Should().Be("2020-05-24");

            schemas.Offset().Type.Should().Be(JsonSchemaType.String);
            schemas.Offset().Format.Should().Be(null);
            schemas.Offset().Example.AsString().Should().Be("+03");

            schemas.Period().Type.Should().Be(JsonSchemaType.String);
            schemas.Period().Format.Should().Be(null);
            schemas.Period().Example.AsString().Should().Be("P1DT1H1M1S1s");

            schemas.Duration().Type.Should().Be(JsonSchemaType.String);
            schemas.Duration().Format.Should().Be(null);
            schemas.Duration().Example.AsString().Should().Be("25:01:01.001");

            schemas.OffsetDate().Type.Should().Be(JsonSchemaType.String);
            schemas.OffsetDate().Format.Should().Be(null);
            schemas.OffsetDate().Example.AsString().Should().Be("2020-05-23+03");

            schemas.OffsetTime().Type.Should().Be(JsonSchemaType.String);
            schemas.OffsetTime().Format.Should().Be(null);
            schemas.OffsetTime().Example.AsString().Should().Be("13:30:50+03");

            schemas.DateTimeZone().Type.Should().Be(JsonSchemaType.String);
            schemas.DateTimeZone().Format.Should().Be(null);
            schemas.DateTimeZone().Example.AsString().Should().Be("Europe/Moscow");
        }
    }

    internal static class TestExtensions
    {
        public static string AsString(this JsonNode openApiAny)
        {
            return openApiAny.ToString();
        }
    }
}
