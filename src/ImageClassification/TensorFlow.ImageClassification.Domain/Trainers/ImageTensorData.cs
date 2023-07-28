using ImageClassification.Domain.Models;
using Tensorflow.NumPy;

namespace ImageClassification.Domain.Trainers;

public struct ImageTensorData
{
    public NDArray NdArray { get; }
    public ImageData ImageData { get; }


    public ImageTensorData(NDArray ndArray, ImageData imageData)
    {
        NdArray = ndArray;
        ImageData = imageData;
    }
}