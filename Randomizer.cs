using System;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint
{
    public void FromVegas(Vegas vegas)
    {
        // Get the currently selected track
        Track selectedTrack = GetSelectedTrack(vegas);
        if (selectedTrack == null)
        {
            MessageBox.Show("Please select a track.");
            return;
        }

        // Check if the track has enabled effects
        if (!HasEnabledEffect(selectedTrack))
        {
            MessageBox.Show("Please select a track with at least one enabled effect.");
            return;
        }

        // Get the first enabled effect to randomize
        //Effect effect = GetFirstEnabledEffect(selectedTrack);
        foreach (Track track in vegas.Project.Tracks)
        {
         	foreach (Effect effect in track.Effects)  // Accessing the effects on the track
            {
                if (!effect.Bypass) // Effect is enabled if Bypass is false
                {
                   ApplyEffect(effect);
                }
            }
        }
        MessageBox.Show("Effect parameters randomized!");
    }

	private void ApplyEffect(Effect effect)
	{
	    Random random = new Random();

		if (effect == null)
        {
            MessageBox.Show("No enabled effects found.");
            return;
        }

        // Create keyframes for the effect with random values
        int keyframeCount = 100; // Define how many keyframes you want
        for (int i = 0; i < keyframeCount; i++)
        {
            // Generate a random time for the keyframe within the project's length
            Timecode randomTime = new Timecode(random.Next(0, 10000));
			//Timecode randomTime = new Timecode(random.Next(0, (int)vegas.Project.Length.ToMilliseconds()));

            // Iterate through each parameter of the effect
            foreach (OFXParameter param in effect.OFXEffect.Parameters)
            {
                if (param.CanAnimate)
                {
                    // Ensure we're working with a double parameter, and manipulate keyframes
                    OFXDoubleParameter doubleParam = param as OFXDoubleParameter;
                    if (doubleParam != null && !doubleParam.Name.ToLower().Contains("mocha"))
                    {
						//MessageBox.Show(doubleParam.Name);
                        // Generate a random value within the parameter's range
                        double randomValue = GenerateRandomValueForParameter(doubleParam, random);

                        // Set the value at the specific random time
                        doubleParam.SetValueAtTime(randomTime, randomValue);
                    }
                }
            }
        }
	}
	
    // Helper function to check if the track has at least one enabled effect
    private bool HasEnabledEffect(Track track)
    {
        foreach (Effect effect in track.Effects)  // Accessing the effects on the track
        {
            if (!effect.Bypass) // Effect is enabled if Bypass is false
            {
                return true;
            }
        }
        return false;
    }

    // Helper function to get the first enabled effect
    private Effect GetFirstEnabledEffect(Track track)
    {
        foreach (Effect effect in track.Effects)  // Accessing the effects on the track
        {
            if (!effect.Bypass) // Effect is enabled if Bypass is false
            {
                return effect;
            }
        }
        return null;
    }

    // Helper function to get the selected track
    private Track GetSelectedTrack(Vegas vegas)
    {
        foreach (Track track in vegas.Project.Tracks)
        {
            if (track.Selected)
            {
                return track;
            }
        }
        return null;
    }

    // Helper function to generate a random value based on parameter type
    private double GenerateRandomValueForParameter(OFXDoubleParameter param, Random random)
    {
        double minValue = -1;
        double maxValue = 1;
        return minValue + (random.NextDouble() * (maxValue - minValue));
    }
}
