using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class ScheduledAction<T>
    {
        // -------------------------------------------------------------------------------------------------

        public delegate bool ScheduledActionCallback(T customData);

        private ScheduledActionCallback m_callback = null;

        private bool m_isActive = false;
        private float m_startTime = 0.0f;
        private float m_duration = 0.0f;
        private int m_runCount = 1;
        private int m_currentCount = 0;
        private T m_customData = default;

        public bool isActive { get { return m_isActive; } }

        // -------------------------------------------------------------------------------------------------

        public void SetCallback(ScheduledActionCallback callback)
        {
            m_callback = callback;
        }

        // -------------------------------------------------------------------------------------------------

        public void Reset()
        {
            m_isActive = false;
            m_startTime = 0.0f;
            m_duration = 0.0f;
            m_runCount = 0;
            m_currentCount = 0;
            m_customData = default;
        }

        // -------------------------------------------------------------------------------------------------

        public bool Schedule(float startTime, float duration, T customData, int count = 1)
        {

            if (m_isActive || m_callback == null)
            {
                return false;
            }

            m_isActive = true;
            m_startTime = startTime;
            m_duration = Mathf.Max(0.0f, duration);
            m_runCount = Mathf.Max(1, count);
            m_currentCount = 0;
            m_customData = customData;
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public void Update(float time)
        {
            if (m_isActive)
            {
                if (time >= m_startTime + m_duration)
                {
                    float t = m_startTime + m_duration;
                    while (t <= time && m_currentCount < m_runCount)
                    {
                        if (m_callback(m_customData) == false)
                        {
                            // if the callback returns false, exit the loop early
                            m_currentCount = m_runCount;
                            break;
                        }

                        m_currentCount++;
                        t += m_duration;
                    }

                    if (m_currentCount >= m_runCount)
                    {
                        Reset();
                    }
                    else
                    {
                        m_startTime = t - m_duration;
                    }
                }
            }
        }

        // -------------------------------------------------------------------------------------------------
    }
}
