using ChatGPT.Wpf.App.Dialogs.Models;
using ChatGPT.Wpf.App.Models;

namespace ChatGPT.Wpf.App.TabPages.Edits;

public class EditsViewModel : BaseViewModel
{

    public CompletionOptionsViewModel Options { get; } = new();

    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();


    public InstructionTextModel Instruction { get; set; } = new() { Text = "Fix the spelling mistakes" };
    public HintTextModel Hint { get; set; } = new() { Text = "Enter Instruction" };


    //private HintTextModel hint;
    //public HintTextModel Hint
    //{
    //    get => hint;
    //    set
    //    {
    //        if (value == hint) return;
    //        hint = value;
    //        OnPropertyChanged(nameof(Hint));
    //    }
    //}

    //private InstructionTextModel instruction;
    //public InstructionTextModel Instruction
    //{
    //    get => instruction;
    //    set
    //    {
    //        if (value == instruction) return;
    //        instruction = value;
    //        OnPropertyChanged(nameof(Instruction));
    //    }
    //}


}
