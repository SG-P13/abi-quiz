using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarController : MonoBehaviour
{
    public Animator sidebarAnimator; // Reference to the Animator component of the Sidebar
    public string animationClipName = "SidebarSlideIn"; // Name of the animation clip

    public void ShowSidebar()
    {
        sidebarAnimator.Play(animationClipName);
    }
}