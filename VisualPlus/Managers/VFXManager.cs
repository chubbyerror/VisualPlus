namespace VisualPlus.Managers
{
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using VisualPlus.Enumerators;

    #endregion

    internal class VFXManager
    {
        #region Variables

        private readonly List<AnimationDirection> animationDirections;
        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };
        private readonly List<object[]> effectsData;
        private readonly List<double> effectsProgression;
        private readonly List<Point> effectsSources;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="VFXManager" /> class.Constructor</summary>
        /// <param name="singular">
        ///     If true, only one animation is supported. The current animation will be replaced with the new
        ///     one. If false, a new animation is added to the list.
        /// </param>
        public VFXManager(bool singular = true)
        {
            effectsProgression = new List<double>();
            effectsSources = new List<Point>();
            animationDirections = new List<AnimationDirection>();
            effectsData = new List<object[]>();

            Increment = 0.03;
            SecondaryIncrement = 0.03;
            EffectType = EffectType.Linear;
            CancelAnimation = true;
            Singular = singular;

            if (Singular)
            {
                effectsProgression.Add(0);
                effectsSources.Add(new Point(0, 0));
                animationDirections.Add(AnimationDirection.In);
            }

            animationTimer.Tick += AnimationTimerOnTick;
        }

        public delegate void AnimationFinished(object sender);

        public delegate void AnimationProgress(object sender);

        public event AnimationFinished OnAnimationFinished;

        public event AnimationProgress OnAnimationProgress;

        #endregion

        #region Properties

        public bool CancelAnimation { get; set; }

        public EffectType EffectType { get; set; }

        public double Increment { get; set; }

        public double SecondaryIncrement { get; set; }

        public bool Singular { get; set; }

        #endregion

        #region Events

        public int GetAnimationCount()
        {
            return effectsProgression.Count;
        }

        public object[] GetData()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsData.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return effectsData[0];
        }

        public object[] GetData(int index)
        {
            if (!(index < effectsData.Count))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return effectsData[index];
        }

        public AnimationDirection GetDirection()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (animationDirections.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return animationDirections[0];
        }

        public AnimationDirection GetDirection(int index)
        {
            if (!(index < animationDirections.Count))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return animationDirections[index];
        }

        public double GetProgress()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsProgression.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return GetProgress(0);
        }

        public double GetProgress(int index)
        {
            if (!(index < GetAnimationCount()))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            switch (EffectType)
            {
                case EffectType.Linear:
                    return AnimationLinear.CalculateProgress(effectsProgression[index]);
                case EffectType.EaseInOut:
                    return AnimationEaseInOut.CalculateProgress(effectsProgression[index]);
                case EffectType.EaseOut:
                    return AnimationEaseOut.CalculateProgress(effectsProgression[index]);
                case EffectType.CustomQuadratic:
                    return AnimationCustomQuadratic.CalculateProgress(effectsProgression[index]);
                default:
                    throw new NotImplementedException("The given EffectType is not implemented");
            }
        }

        public Point GetSource(int index)
        {
            if (!(index < GetAnimationCount()))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return effectsSources[index];
        }

        public Point GetSource()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsSources.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return effectsSources[0];
        }

        public bool IsAnimating()
        {
            return animationTimer.Enabled;
        }

        public void SetData(object[] data)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsData.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            effectsData[0] = data;
        }

        public void SetDirection(AnimationDirection direction)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsProgression.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            animationDirections[0] = direction;
        }

        public void SetProgress(double progress)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (effectsProgression.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            effectsProgression[0] = progress;
        }

        public void StartNewAnimation(AnimationDirection animationDirection, object[] data = null)
        {
            StartNewAnimation(animationDirection, new Point(0, 0), data);
        }

        public void StartNewAnimation(AnimationDirection animationDirection, Point animationSource, object[] data = null)
        {
            if (!IsAnimating() || CancelAnimation)
            {
                if (Singular && (animationDirections.Count > 0))
                {
                    animationDirections[0] = animationDirection;
                }
                else
                {
                    animationDirections.Add(animationDirection);
                }

                if (Singular && (effectsSources.Count > 0))
                {
                    effectsSources[0] = animationSource;
                }
                else
                {
                    effectsSources.Add(animationSource);
                }

                if (!(Singular && (effectsProgression.Count > 0)))
                {
                    switch (animationDirections[animationDirections.Count - 1])
                    {
                        case AnimationDirection.InOutRepeatingIn:
                        case AnimationDirection.InOutIn:
                        case AnimationDirection.In:
                            effectsProgression.Add(MinValue);
                            break;
                        case AnimationDirection.InOutRepeatingOut:
                        case AnimationDirection.InOutOut:
                        case AnimationDirection.Out:
                            effectsProgression.Add(MaxValue);
                            break;
                        default:
                            throw new Exception("Invalid AnimationDirection");
                    }
                }

                if (Singular && (effectsData.Count > 0))
                {
                    effectsData[0] = data ?? new object[] { };
                }
                else
                {
                    effectsData.Add(data ?? new object[] { });
                }
            }

            animationTimer.Start();
        }

        public void UpdateProgress(int index)
        {
            switch (animationDirections[index])
            {
                case AnimationDirection.InOutRepeatingIn:
                case AnimationDirection.InOutIn:
                case AnimationDirection.In:
                    IncrementProgress(index);
                    break;
                case AnimationDirection.InOutRepeatingOut:
                case AnimationDirection.InOutOut:
                case AnimationDirection.Out:
                    DecrementProgress(index);
                    break;
                default:
                    throw new Exception("No AnimationDirection has been set");
            }
        }

        private const double MaxValue = 1.00;
        private const double MinValue = 0.00;

        private void AnimationTimerOnTick(object sender, EventArgs eventArgs)
        {
            for (var i = 0; i < effectsProgression.Count; i++)
            {
                UpdateProgress(i);

                if (!Singular)
                {
                    if ((animationDirections[i] == AnimationDirection.InOutIn) && (effectsProgression[i] == MaxValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutOut;
                    }
                    else if ((animationDirections[i] == AnimationDirection.InOutRepeatingIn) && (effectsProgression[i] == MinValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                    }
                    else if ((animationDirections[i] == AnimationDirection.InOutRepeatingOut) && (effectsProgression[i] == MinValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                    }
                    else if (((animationDirections[i] == AnimationDirection.In) && (effectsProgression[i] == MaxValue))
                             || ((animationDirections[i] == AnimationDirection.Out) && (effectsProgression[i] == MinValue))
                             || ((animationDirections[i] == AnimationDirection.InOutOut) && (effectsProgression[i] == MinValue)))
                    {
                        effectsProgression.RemoveAt(i);
                        effectsSources.RemoveAt(i);
                        animationDirections.RemoveAt(i);
                        effectsData.RemoveAt(i);
                    }
                }
                else
                {
                    if ((animationDirections[i] == AnimationDirection.InOutIn) && (effectsProgression[i] == MaxValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutOut;
                    }
                    else if ((animationDirections[i] == AnimationDirection.InOutRepeatingIn) && (effectsProgression[i] == MaxValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                    }
                    else if ((animationDirections[i] == AnimationDirection.InOutRepeatingOut) && (effectsProgression[i] == MinValue))
                    {
                        animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                    }
                }
            }

            OnAnimationProgress?.Invoke(this);
        }

        private void DecrementProgress(int index)
        {
            effectsProgression[index] -= (animationDirections[index] == AnimationDirection.InOutOut)
                                         || (animationDirections[index] == AnimationDirection.InOutRepeatingOut)
                                             ? SecondaryIncrement
                                             : Increment;
            if (effectsProgression[index] < MinValue)
            {
                effectsProgression[index] = MinValue;

                for (var i = 0; i < GetAnimationCount(); i++)
                {
                    if (animationDirections[i] == AnimationDirection.InOutIn)
                    {
                        return;
                    }

                    if (animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                    {
                        return;
                    }

                    if (animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                    {
                        return;
                    }

                    if ((animationDirections[i] == AnimationDirection.InOutOut) && (effectsProgression[i] != MinValue))
                    {
                        return;
                    }

                    if ((animationDirections[i] == AnimationDirection.Out) && (effectsProgression[i] != MinValue))
                    {
                        return;
                    }
                }

                animationTimer.Stop();
                OnAnimationFinished?.Invoke(this);
            }
        }

        private void IncrementProgress(int index)
        {
            effectsProgression[index] += Increment;
            if (effectsProgression[index] > MaxValue)
            {
                effectsProgression[index] = MaxValue;

                for (var i = 0; i < GetAnimationCount(); i++)
                {
                    if (animationDirections[i] == AnimationDirection.InOutIn)
                    {
                        return;
                    }

                    if (animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                    {
                        return;
                    }

                    if (animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                    {
                        return;
                    }

                    if ((animationDirections[i] == AnimationDirection.InOutOut) && (effectsProgression[i] != MaxValue))
                    {
                        return;
                    }

                    if ((animationDirections[i] == AnimationDirection.In) && (effectsProgression[i] != MaxValue))
                    {
                        return;
                    }
                }

                animationTimer.Stop();
                OnAnimationFinished?.Invoke(this);
            }
        }

        #endregion
    }

    internal class AnimationLinear
    {
        #region Events

        public static double CalculateProgress(double progress)
        {
            return progress;
        }

        #endregion
    }

    internal class AnimationEaseInOut
    {
        #region Events

        public static double CalculateProgress(double progress)
        {
            return EaseInOut(progress);
        }

        public static double Pi = Math.PI;
        public static double PiHalf = Math.PI / 2;

        private static double EaseInOut(double s)
        {
            return s - Math.Sin((s * 2 * Pi) / (2 * Pi));
        }

        #endregion
    }

    public static class AnimationEaseOut
    {
        #region Events

        public static double CalculateProgress(double progress)
        {
            return -1 * progress * (progress - 2);
        }

        #endregion
    }

    public static class AnimationCustomQuadratic
    {
        #region Events

        public static double CalculateProgress(double progress)
        {
            const double Boost = 0.6;
            return 1 - Math.Cos(((Math.Max(progress, Boost) - Boost) * Math.PI) / (2 - (2 * Boost)));
        }

        #endregion
    }
}