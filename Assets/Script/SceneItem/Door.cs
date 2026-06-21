using UnityEngine;

public class Door : MonoBehaviour, ItemInterface
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private AnimationClip closeClip;
    private bool isOpen;
    private bool isPlaying;
    private float animEndTime;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isPlaying && Time.time >= animEndTime)
            isPlaying = false;
    }

    public void UseItem()
    {
        if (isPlaying) return;
        isOpen = !isOpen;
        AnimationClip clip = isOpen ? openClip : closeClip;
        animator.Play(clip.name);
        animEndTime = Time.time + clip.length;
        isPlaying = true;
    }
}
