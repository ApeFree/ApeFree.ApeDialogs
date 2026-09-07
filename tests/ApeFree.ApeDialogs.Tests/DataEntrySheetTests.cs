using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using Xunit;

namespace ApeFree.ApeDialogs.Tests
{
    public class DataEntrySheetTests
    {
        [Fact]
        public void AddField_And_IndexAccess()
        {
            var sheet = new DataEntrySheet();
            var field = new TextField { Title = "Name" };
            sheet.AddField(field);

            Assert.Single(sheet.Fields);
            Assert.Same(field, sheet[0]);
            Assert.Same(field, sheet["Name"]);
            Assert.Null(sheet["Missing"]);
        }

        [Fact]
        public void AddField_Throws_OnDuplicateTitle()
        {
            var sheet = new DataEntrySheet();
            sheet.AddField(new TextField { Title = "Name" });

            Assert.Throws<ArgumentException>(() => sheet.AddField(new TextField { Title = "Name" }));
        }

        [Fact]
        public void AddField_Throws_OnEmptyTitle()
        {
            var sheet = new DataEntrySheet();
            Assert.Throws<ArgumentException>(() => sheet.AddField(new TextField()));
        }

        [Fact]
        public void AddField_ReturnsSheet_ForFluentChaining()
        {
            var sheet = new DataEntrySheet();
            var result = sheet.AddField(new TextField { Title = "A" }).AddField(new TextField { Title = "B" });

            Assert.Same(sheet, result);
            Assert.Equal(2, sheet.Fields.Count);
        }

        [Fact]
        public void TypedField_Value_RoundTrip()
        {
            var field = new TextField { Title = "T", Value = "hello" };
            Assert.Equal("hello", field.Value);
            Assert.Equal("hello", field.GetRawValue());

            field.SetRawValue("world");
            Assert.Equal("world", field.Value);
        }

        [Fact]
        public void TypedField_Value_HandlesNull()
        {
            var field = new TextField { Title = "T", Value = null };
            Assert.Null(field.Value);
        }

        [Fact]
        public void Field_Validate_WithoutValidator_Succeeds()
        {
            var field = new TextField { Title = "T" };
            Assert.True(field.Validate().IsSuccess);
        }

        [Fact]
        public void Field_Validate_UsesValidator()
        {
            var field = new TextField { Title = "T" };
            field = new TextField
            {
                Title = "T",
                Validator = v => string.IsNullOrEmpty(v) ? ValidationResult.Failure("empty") : ValidationResult.Success,
            };

            Assert.False(field.Validate().IsSuccess);
            field.Value = "ok";
            Assert.True(field.Validate().IsSuccess);
        }

        [Fact]
        public void Sheet_Validate_ReturnsFirstFailure()
        {
            var sheet = new DataEntrySheet();
            sheet.AddField(new TextField
            {
                Title = "A",
                Validator = _ => ValidationResult.Success,
            });
            sheet.AddField(new TextField
            {
                Title = "B",
                Validator = _ => ValidationResult.Failure("bad B"),
            });
            sheet.AddField(new TextField
            {
                Title = "C",
                Validator = _ => ValidationResult.Failure("bad C"),
            });

            var result = sheet.Validate();
            Assert.False(result.IsSuccess);
            Assert.Equal("bad B", result.ErrorMessage);
        }

        [Fact]
        public void Sheet_Validate_AllSuccess()
        {
            var sheet = new DataEntrySheet();
            sheet.AddField(new NumberField { Title = "N" });

            Assert.True(sheet.Validate().IsSuccess);
        }

        [Fact]
        public void FieldTypes_AreExposed()
        {
            Assert.Equal(FieldType.Text, new TextField().FieldType);
            Assert.Equal(FieldType.Password, new PasswordField().FieldType);
            Assert.Equal(FieldType.HexBytes, new HexBytesField().FieldType);
            Assert.Equal(FieldType.FilePath, new FilePathField().FieldType);
            Assert.Equal(FieldType.PicturePath, new PicturePathField().FieldType);
            Assert.Equal(FieldType.Number, new NumberField().FieldType);
            Assert.Equal(FieldType.SingleChoice, new SingleChoiceField().FieldType);
            Assert.Equal(FieldType.ComboBox, new ComboBoxField().FieldType);
            Assert.Equal(FieldType.MultipleChoice, new MultipleChoiceField().FieldType);
            Assert.Equal(FieldType.DateTime, new DateTimeField().FieldType);
        }

        [Fact]
        public void DateTimeField_FormatIs24Hour()
        {
            Assert.Contains("HH", new DateTimeField().DateTimeFormat);
            Assert.DoesNotContain("hh", new DateTimeField().DateTimeFormat);
        }

        [Fact]
        public void SheetField_Generic_IsValueTyped()
        {
            var field = new NumberField { Title = "N", Value = 3.14m };
            Assert.Equal(3.14m, field.Value);
        }

        [Fact]
        public void AddXxxField_FluentChaining_AllTypesSupported()
        {
            var sheet = new DataEntrySheet()
                .AddTextField("Name", "ZhangSan")
                .AddPasswordField(p => { p.Title = "Password"; p.Value = "123456"; })
                .AddNumberField("Age", 18, minimum: 0, maximum: 120, decimalPlaces: 0)
                .AddDateTimeField("Birthday", new DateTime(2000, 1, 1))
                .AddFilePathField("Doc", browseButtonText: "Select Doc...")
                .AddPicturePathField(p => { p.Title = "Photo"; p.BrowseButtonText = "Select Photo..."; })
                .AddHexBytesField("Bytes", new byte[] { 0x01, 0x02 })
                .AddComboBoxField("City", new[] { "Beijing", "Shanghai" }, "Beijing")
                .AddSingleChoiceField("Gender", new[] { "Male", "Female" }, "Male")
                .AddMultipleChoiceField("Tags", new[] { "A", "B", "C" }, new[] { "A", "B" });

            Assert.Equal(10, sheet.Fields.Count);
            Assert.IsType<TextField>(sheet["Name"]);
            Assert.IsType<PasswordField>(sheet["Password"]);
            Assert.IsType<NumberField>(sheet["Age"]);
            Assert.IsType<DateTimeField>(sheet["Birthday"]);
            Assert.IsType<FilePathField>(sheet["Doc"]);
            Assert.IsType<PicturePathField>(sheet["Photo"]);
            Assert.IsType<HexBytesField>(sheet["Bytes"]);
            Assert.IsType<ComboBoxField>(sheet["City"]);
            Assert.IsType<SingleChoiceField>(sheet["Gender"]);
            Assert.IsType<MultipleChoiceField>(sheet["Tags"]);
        }
    }
}
