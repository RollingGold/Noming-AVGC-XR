using UnityEngine.InputSystem;

public static class InputBindingUtility
{
    public static string GetPreferredBinding(InputAction action)
    {
        // Mouse first
        foreach (var binding in action.bindings)
        {
            if (binding.path.Contains("<Mouse>/leftButton"))
                return "LMB";

            if (binding.path.Contains("<Mouse>/rightButton"))
                return "RMB";

            if (binding.path.Contains("<Mouse>/middleButton"))
                return "MMB";
        }

        // Keyboard second
        foreach (var binding in action.bindings)
        {
            if (binding.path.Contains("<Keyboard>"))
            {
                return FormatBinding(
                    binding.ToDisplayString());
            }
        }

        return "";
    }

    private static string FormatBinding(string binding)
    {
        switch (binding)
        {
            case "Left Mouse":
                return "LMB";

            case "Right Mouse":
                return "RMB";

            case "Middle Mouse":
                return "MMB";

            case "Left Shift":
                return "SHIFT";

            case "Space":
                return "SPACE";

            default:
                return binding.ToUpper();
        }
    }
}