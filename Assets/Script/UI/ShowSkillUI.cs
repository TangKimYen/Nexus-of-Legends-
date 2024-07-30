using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSkillUI : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public Image skillImage;
        public TMP_Text cooldownText;
        public float cooldownTime;
        [HideInInspector] public float cooldownRemaining;
    }

    public List<Skill> skills = new List<Skill>();

    void Start()
    {
        foreach (var skill in skills)
        {
            skill.cooldownText.text = "";
        }
    }

    void Update()
    {
        foreach (var skill in skills)
        {
            if (skill.cooldownRemaining > 0)
            {
                skill.cooldownRemaining -= Time.deltaTime;
                skill.cooldownText.text = Mathf.Ceil(skill.cooldownRemaining).ToString();
                skill.skillImage.fillAmount = skill.cooldownRemaining / skill.cooldownTime;
            }
            else
            {
                skill.cooldownText.text = "";
                skill.skillImage.fillAmount = 1;
            }
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Count)
            return;

        if (skills[skillIndex].cooldownRemaining <= 0)
        {
            skills[skillIndex].cooldownRemaining = skills[skillIndex].cooldownTime;
            // Add your skill logic here
            Debug.Log("Skill " + (skillIndex + 1) + " used.");
        }
    }
}
