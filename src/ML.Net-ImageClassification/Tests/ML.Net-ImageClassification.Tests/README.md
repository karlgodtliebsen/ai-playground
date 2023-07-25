# A ML.Net Image Classification 

This project contains some test Implementation of ML.Net Image Classification, using tenstorflow.

Based on :
https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training
https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training/ImageClassification.Train/Program.cs#L135
https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training/ImageClassification.Predict/Program.cs
 

Dataset used are:

Flowers:
http://download.tensorflow.org/example_images/flower_photos.tgz

Meat Quality Assessment Dataset:
https://www.kaggle.com/datasets/crowww/meat-quality-assessment-based-on-deep-learning
    
```
@inproceedings{
ulucan2019meat,
title={Meat quality assessment based on deep learning},
author={Ulucan, Oguzhan and Karakaya, Diclehan and Turkan, Mehmet},
booktitle={2019 Innovations in Intelligent Systems and Applications Conference (ASYU)},
pages={1--5},
year={2019},
organization={IEEE}
}    
```

## How to use
Adjust the appsettings.IntegrationTests.json file to point to the path where your datasets are located.

```

  "ImageClassification": {
    "imageFilePath": "/projects/AI/Images/{imageSetPath}/inputs/images",
    "outputFilePath": "/projects/AI/Images/{imageSetPath}/outputs",
    "inputFilePath": "/projects/AI/Images/{imageSetPath}/inputs"
  },
```


##Sample dataset location:

```
/somepath/Images/flower_photos/inputs/images
/somepath/Images/flower_photos/inputs/test-images
/somepath/Images/flower_photos/outputs
```



