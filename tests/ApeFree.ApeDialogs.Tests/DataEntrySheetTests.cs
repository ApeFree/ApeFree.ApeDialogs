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

        [Fact]
        public void NumberField_BuiltInValidation_EnforcesMinMax()
        {
            var field = new NumberField { Minimum = 10, Maximum = 100, Value = 5 };
            var result = field.Validate();
            Assert.False(result.IsSuccess);

            field.Value = 50;
            Assert.True(field.Validate().IsSuccess);

            field.Value = 101;
            Assert.False(field.Validate().IsSuccess);
        }

        [Fact]
        public void TextField_And_PasswordField_BuiltInValidation_EnforcesMaximumLength()
        {
            var textField = new TextField { MaximumLength = 5, Value = "123456" };
            Assert.False(textField.Validate().IsSuccess);

            textField.Value = "12345";
            Assert.True(textField.Validate().IsSuccess);

            var passwordField = new PasswordField { MaximumLength = 6, Value = "abcdefg" };
            Assert.False(passwordField.Validate().IsSuccess);

            passwordField.Value = "abcdef";
            Assert.True(passwordField.Validate().IsSuccess);
        }

        [Fact]
        public void HexBytesField_BuiltInValidation_EnforcesAllowEmptyAndLengths()
        {
            var field = new HexBytesField { AllowEmpty = false, Value = null };
            Assert.False(field.Validate().IsSuccess);

            field.Value = Array.Empty<byte>();
            Assert.False(field.Validate().IsSuccess);

            field.MinimumLength = 2;
            field.MaximumLength = 4;
            field.Value = new byte[] { 0x01 };
            Assert.False(field.Validate().IsSuccess);

            field.Value = new byte[] { 0x01, 0x02, 0x03 };
            Assert.True(field.Validate().IsSuccess);

            field.Value = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            Assert.False(field.Validate().IsSuccess);
        }

        [Fact]
        public void BuiltInValidation_ChainsWithCustomValidator()
        {
            var field = new NumberField
            {
                Minimum = 0,
                Maximum = 100,
                Value = 15,
                Validator = val => val % 2 == 0 ? ValidationResult.Success : ValidationResult.Failed("Must be even.")
            };

            // 虽在 [0, 100] 范围内，但自定义验证未通过
            var result = field.Validate();
            Assert.False(result.IsSuccess);
            Assert.Equal("Must be even.", result.ErrorMessage);

            // 超出内置范围时，优先被内置校验拦截
            field.Value = 102;
            var outOfRangeResult = field.Validate();
            Assert.False(outOfRangeResult.IsSuccess);
            Assert.Equal(DialogLocalization.ValueOutOfRange(0, 100), outOfRangeResult.ErrorMessage);
        }
    }
}
