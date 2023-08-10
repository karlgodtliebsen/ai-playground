# TensorFlow Image Classification 
This project contains some test Implementation of TensorFlow.Net and TensorFlow.Keras Image Classification.

### Based on :
- https://github.com/SciSharp/TensorFlow.NET
- https://github.com/dotnet/samples/
 
https://scisharp.github.io/tensorflow-net-docs/#/tutorials/ImageRecognition?id=_1-prepare-data


### Dataset used are from:

- https://www.tensorflow.org/datasets
- https://www.tensorflow.org/datasets/catalog/overview
- https://github.com/tensorflow/datasets/tree/master/tensorflow_datasets/datasets
- http://www.vision.caltech.edu/datasets/

>Caltect101:
- https://data.caltech.edu/records/mzrjq-6wc02
> Stanford dogs:
- http://vision.stanford.edu/aditya86/ImageNetDogs/main.html
- 
>Flowers:
- http://download.tensorflow.org/example_images/flower_photos.tgz
- https://storage.googleapis.com/download.tensorflow.org/example_images/flower_photos.tgz
- https://www.robots.ox.ac.uk/~vgg/data/flowers/102/

> Meat Quality Assessment Dataset:
- https://www.kaggle.com/datasets/crowww/meat-quality-assessment-based-on-deep-learning

> Butterfly Image Classification:
- https://www.kaggle.com/datasets/phucthaiv02/butterfly-image-classification
- https://s3.us-west-1.wasabisys.com/dphi/public-datasets/Data_Sprint_107_Butterfly_Classification/butterflies.zip
> Food Image Classification Dataset: 
- https://www.kaggle.com/datasets/harishkumardatalab/food-image-classification-dataset
> Stanford Cars Dataset:
- https://www.kaggle.com/datasets/jessicali9530/stanford-cars-dataset
- https://github.com/BotechEngineering/StanfordCarsDatasetCSV/tree/main
> Animals-10
- https://www.kaggle.com/datasets/alessiocorrado99/animals10
> Cats and Dogs
- https://www.kaggle.com/datasets/samuelcortinhas/cats-and-dogs-image-classification
> Animal Image Dataset (90 Different Animals)
- https://www.kaggle.com/datasets/iamsouravbanerjee/animal-image-dataset-90-different-animals
> Fashion Product Images (Small)
- https://www.kaggle.com/paramaggarwal/fashion-product-images-small

### Citations  

```
Cars:
- 3D Object Representations for Fine-Grained Categorization
- Jonathan Krause, Michael Stark, Jia Deng, Li Fei-Fei
- 4th IEEE Workshop on 3D Representation and Recognition, at ICCV 2013 (3dRR-13). Sydney, Australia. Dec. 8, 2013.

Meat:
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
{
  "ImageClassification": {
    "trainImagesFilePath": "/projects/AI/Images/{imageSetPath}/inputs/train-images",
    "testImagesFilePath": "/projects/AI/Images/{imageSetPath}/inputs/test-images",
    "outputFilePath": "/projects/AI/Images/{imageSetPath}/outputs",
    "inputFilePath": "/projects/AI/Images/{imageSetPath}/inputs",
    "modelName": "ImageClassifierModel.zip"
  }
```


## Sample dataset location:

```
/somepath/Images/animals/inputs/train-images
/somepath/Images/animals/inputs/train-images/dogs/OIF-e2bexWrojgtQnAPPcUfOWQ.jpeg
/somepath/Images/animals/inputs/train-images/cat/1.jpeg

/somepath/Images/animals/inputs/test-images
/somepath/Images/animals/inputs/test-images/dogs/42.jpeg
/somepath/Images/animals/inputs/test-images/cat/14242.jpeg

/somepath/Images/animals/outputs
/somepath/Images/animals/outputs/ImageClassifierModel.zip
```



