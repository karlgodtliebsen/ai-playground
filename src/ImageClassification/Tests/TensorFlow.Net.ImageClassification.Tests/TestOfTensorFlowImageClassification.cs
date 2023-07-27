using AI.Test.Support;

using ML.Net.ImageClassification.Tests.Domain;
using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;

namespace ML.Net.ImageClassification.Tests;

//Based on: 

[Collection("TensorFlow Image Classification Collection")]
public class TestOfTensorFlowImageClassification : TestFixtureBase
{
    private readonly TensorFlowImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfTensorFlowImageClassification(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }

    //birds: birds.csv: 2.0,train/ABYSSINIAN GROUND HORNBILL/141.jpg,ABYSSINIAN GROUND HORNBILL,train,BUCORVUS ABYSSINICUS
    //butterfly: Training_set.csv: Image_1.jpg,SOUTHERN DOGFACE
    //butterfly: Testing_set.csv: Image_1.jpg
    //cars: MATLAB 5.0 MAT-file, Platform: GLNXA64, Created on: Sat Feb 28 19:34:55 2015 
    //fachion products: 29114,Men,Accessories,Socks,Socks,Navy Blue,Summer,2012,Casual,Puma Men Pack of 3 Socks

    [Theory]
    [InlineData("flowers")]
    //[InlineData("meat")]
    //[InlineData("butterfly", 0, 1, "Training_set.csv")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("food")]
    //[InlineData("animals")]
    //[InlineData("cars", 5, 6, "cardatasettrain.csv")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    //[InlineData("fashionproducts", 0, "1-9", "styles.csv")]
    public void TrainImageClassificationAndPersistModel(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = CrateImageToLabelMapper(imageIndex, labelIndex, fileName);
        }
        logger.Information("Training [{set}]", dataSet);
        //ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITrainer>();
        //var model = trainer.TrainModel(dataSet, mapper);
        //model.Should().NotBeNull();
    }

    private ImageLabelMapper? CrateImageToLabelMapper(int imageIndex, object? labelIndex, string fileName)
    {
        int? index = default;
        (int from, int to) labelIndexRange = default;
        if (labelIndex is not null)
        {
            if (int.TryParse(labelIndex!.ToString(), out int result))
            {
                index = result;
            }
            else
            {
                var indx = labelIndex!.ToString()!.Split('-');
                if (indx.Length == 2)
                {
                    labelIndexRange = (int.Parse(indx[0]), int.Parse(indx[1]));
                }
                else
                {
                    throw new ArgumentException("Invalid label index range", nameof(labelIndex));
                }
            }
        }
        var mapper = new ImageLabelMapper(fileName, imageIndex!, labelIndex: index, labelIndexRange);
        return mapper;
    }


    //[Theory]
    //[InlineData("flowers")]
    //[InlineData("meat")]
    ////    [InlineData("butterfly", 0, 0, "Testing_set.csv")]
    //[InlineData("food")]
    //[InlineData("animals")]
    ////    [InlineData("cars", 5, 6, "cardatasettest.csv")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    ////    [InlineData("fashionproducts", 0, "1-9", "styles.csv")]
    //public void UsePersistedImageClassificationModelToPredictImages(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    //{
    //    ImageLabelMapper? mapper = null;
    //    if (fileName is not null)
    //    {
    //        mapper = CrateImageToLabelMapper(imageIndex, labelIndex, fileName);
    //    }
    //    logger.Information("Verifying [{set}]", dataSet);
    //    //IPredictor predictor = fixture.Factory.Services.GetRequiredService<IPredictor>();
    //    //predictor.PredictImages(dataSet, mapper);
    //}
}
