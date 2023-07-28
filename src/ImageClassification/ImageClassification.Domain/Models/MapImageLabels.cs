namespace ImageClassification.Domain.Models;

public static class MapImageLabels
{
    public static ImageLabelMapper? CrateImageToLabelMapper(int imageIndex, object? labelIndex, string fileName)
    {
        int? index = default;
        (int from, int to) labelIndexRange = default;
        if (labelIndex is not null)
        {
            if (int.TryParse(labelIndex!.ToString(), out var result))
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
}
