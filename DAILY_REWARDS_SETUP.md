# 🎁 Daily Rewards Setup Guide

Complete guide to implement and configure the Daily Rewards system.

---

## 📊 What is Daily Rewards?

**Daily Rewards** encourage players to log in every day by giving them free rewards.

### Why It's Critical:
- ⬆️ **+40-60% retention** (D1, D7, D30)
- 🎯 Creates daily habit
- 💎 Gives free premium currency
- 🔥 Streak system = FOMO (Fear of Missing Out)
- 📈 More engagement = more monetization

---

## 🎁 Reward Schedule (7 Days)

```
Day 1: Welcome Bonus 🎁
  → 500$ + 20 Food
  "Start your adventure!"

Day 2: Resource Pack 📦
  → 1000$ + 50 Food + 50 Materials
  "Build your prison"

Day 3: FREE CRYSTALS! 💎
  → 25 Crystals
  "Premium currency for free!"

Day 4: Jackpot 💰
  → 2000$ + 100 Food + 100 Materials
  "Big money bonus"

Day 5: More Crystals 💎
  → 1500$ + 50 Crystals
  "Even more premium!"

Day 6: Mega Pack 🎁
  → 3000$ + 200 Food + 150 Materials + 25 Crystals
  "Almost there!"

Day 7: ULTIMATE REWARD 🏆
  → 5000$ + 300 Food + 250 Materials + 100 Crystals
  + Exclusive Skin!
  "7 days streak achievement!"
```

**After Day 7:** Loop back to Day 1 (keeps giving rewards forever!)

---

## 🔥 Streak System

```
Player logs in Day 1: Streak = 1
Player logs in Day 2: Streak = 2
Player logs in Day 3: Streak = 3
...
Player MISSES Day 4: Streak RESETS to 0 ❌
Player logs in Day 5: Streak = 1 (starts over)
```

**Why Streaks Work:**
- Creates urgency ("Don't break the streak!")
- FOMO psychology
- Daily habit formation
- More valuable rewards as streak increases

---

## 💎 Value Analysis

### Total Value of 7-Day Cycle:

```
Money: 13,000$
Food: 720
Materials: 600
Crystals: 200 (worth ~$2-3 in IAP!)

If player completes 4 cycles/month:
→ 800 free crystals/month
→ Equivalent to $8-10 of IAP value
→ BUT they still need to pay for convenience!
```

**Perfect Balance:**
- Generous enough to feel rewarding
- Not so much that it kills monetization
- Creates loyalty without cannibalizing sales

---

## 🛠️ Implementation in Unity

### 1. Add Managers to Scene

```
1. Create Empty GameObject "DailyRewardManager"
2. Add Component: DailyRewardManager.cs
3. Create Empty GameObject "AnalyticsManager"
4. Add Component: AnalyticsManager.cs
```

### 2. Create UI Popup

```
1. GameObject → UI → Panel (name it "DailyRewardPanel")
2. Add:
   - Title Text: "Daily Reward!"
   - Description Text
   - Streak Text: "Streak: X days 🔥"
   - Reward Items Container
   - Day Indicators (1-7)
   - Claim Button
   - Close Button

3. Add DailyRewardUI.cs component
4. Assign references in Inspector
5. Set panel inactive by default
```

### 3. Show Popup on Login

```csharp
// In your main menu or game start script

void Start()
{
    // Check if daily reward is available
    if (DailyRewardManager.Instance.CanClaimToday())
    {
        DailyRewardUI.Instance.ShowDailyRewardPanel();
    }
}
```

### 4. Optional: Add Daily Reward Button

```
Add a button in main menu:
"🎁 Daily Reward"

Button onClick:
DailyRewardUI.Instance.ShowDailyRewardPanel();
```

---

## 📱 User Flow

```
1. Player opens game
   ↓
2. DailyRewardManager checks:
   - Have they claimed today? (Yes → Nothing)
   - Is this first claim ever? (Yes → Show Day 1)
   - Did they break streak? (Yes → Reset to Day 1)
   - Is streak active? (Yes → Show next day)
   ↓
3. Popup appears automatically
   ↓
4. Player sees:
   - What day they're on (1-7)
   - Their current streak
   - Today's rewards
   - Progress indicators
   ↓
5. Player clicks "Claim"
   ↓
6. Rewards added to inventory
   - Money added
   - Food added
   - Materials added
   - Crystals added (via MonetizationManager)
   ↓
7. Success notification
   ↓
8. Popup closes
```

---

## 🎨 UI Design Recommendations

### Colors:
```
Background: Dark gradient (matches game theme)
Accent: Gold/Orange (premium feeling)
Claimed days: Green ✅
Current day: Yellow/Orange 🔥
Future days: Gray/Disabled ⭕
```

### Animations:
```
- Popup slides in from top
- Rewards fade in one by one
- Sparkle effect on crystals
- Confetti on Day 7
- Button pulse effect
```

### Sounds:
```
- Popup open: Light chime
- Reward claim: Success jingle
- Day 7: Special fanfare
```

---

## 📊 Analytics Tracking

The system automatically tracks:

```csharp
// When reward is claimed
AnalyticsManager.TrackEvent("daily_reward_claimed", {
    "streak_day": 3,
    "reward_day": 3,
    "crystals_earned": 25
});

// When streak is broken
AnalyticsManager.TrackEvent("streak_broken", {
    "broken_at_day": 5
});

// Login tracking
AnalyticsManager.TrackEvent("daily_login", {
    "consecutive_days": 7
});
```

**Use this data to:**
- See average streak length
- Find drop-off points
- Optimize reward values
- A/B test different rewards

---

## ⚙️ Configuration (Easy!)

### Change Reward Values:

```csharp
// In DailyRewardManager.cs → InitializeDailyRewards()

// Day 1 example:
dailyRewards.Add(new DailyReward
{
    day = 1,
    rewardName = "Welcome Bonus",
    description = "Start your adventure!",
    icon = "🎁",
    money = 500,      // ← Change this
    food = 20,        // ← Change this
    materials = 0,    // ← Change this
    crystals = 0      // ← Change this
});
```

### Adjust Schedule:

```csharp
// Change number of days in cycle
[SerializeField] private int maxStreakDays = 7; // Change to 30 for monthly

// Disable daily rewards temporarily
[SerializeField] private bool enableDailyRewards = false;
```

---

## 🧪 Testing

### Test Commands (in DailyRewardManager):

```csharp
[ContextMenu("Reset Daily Rewards")]
→ Resets progress, can claim again

[ContextMenu("Simulate Next Day")]
→ Simulates time passing, triggers next reward

[ContextMenu("Test Streak Break")]
→ Simulates missing 2+ days, resets streak
```

**How to use:**
1. Select DailyRewardManager in Hierarchy
2. Right-click component in Inspector
3. Click context menu option

---

## 💡 Best Practices

### DO ✅

1. **Show immediately on app open**
   - Can't miss it
   - Feels like a gift

2. **Make it visually appealing**
   - Colorful
   - Animated
   - Celebration feel

3. **Clear progress indication**
   - Show which day they're on
   - Show streak count
   - Preview future rewards

4. **Give premium currency**
   - Day 3, 5, 7 have crystals
   - Gives taste of premium
   - Encourages purchase later

5. **Big Day 7 reward**
   - Makes it worth the effort
   - Exclusive item = special feeling

### DON'T ❌

1. **Don't hide it**
   - Should be obvious
   - Auto-popup is best

2. **Don't be stingy**
   - Generous = loyalty
   - Cheap = feels insulting

3. **Don't punish too hard for breaking streak**
   - Just reset to Day 1
   - Don't take away items
   - Keep it positive

4. **Don't make it complicated**
   - Simple = better
   - Just log in = reward

---

## 📈 Expected Results

### Industry Average:

```
Without Daily Rewards:
- D1 Retention: 40%
- D7 Retention: 15%
- D30 Retention: 5%

With Daily Rewards:
- D1 Retention: 50-60% (+25%)
- D7 Retention: 25-30% (+66%)
- D30 Retention: 10-15% (+100%)
```

### Your Game:

```
Month 1 (10,000 players):
- Without: 500 return Day 30
- With: 1,000 return Day 30
→ 500 MORE active players!

More players = More engagement = More monetization
```

---

## 🔗 Integration with Other Systems

### Works with:

**Achievements:**
```
"7-Day Streak" achievement
→ Unlock at first Day 7 completion
→ Bonus 100 crystals
```

**Push Notifications:**
```
"Come back tomorrow for your Day 4 reward!"
→ Reminds to maintain streak
```

**VIP System:**
```
VIP players get:
- 2x daily reward value
- Streak insurance (1 free skip/week)
```

**Events:**
```
"Double Daily Rewards Weekend!"
→ Everything x2 for 3 days
→ Huge engagement spike
```

---

## 🚀 Launch Strategy

### Soft Launch (Week 1):
```
→ Enable daily rewards
→ Monitor claim rate
→ Track retention impact
→ Collect feedback
```

### Optimize (Week 2-4):
```
→ Adjust reward values if needed
→ A/B test different rewards
→ Add polish (animations, sounds)
→ Fix any bugs
```

### Full Launch:
```
→ Announce in patch notes
→ Push notification reminder
→ Track long-term retention
→ Celebrate success! 🎉
```

---

## 📚 Resources

- [Daily Rewards Best Practices](https://www.gameanalytics.com/blog/daily-rewards/)
- [Retention Strategies](https://www.gamedeveloper.com/business/retention)
- [Psychology of Daily Rewards](https://www.deconstructoroffun.com/)

---

## ✅ Checklist

Implementation complete when:

- [ ] DailyRewardManager in scene
- [ ] AnalyticsManager in scene
- [ ] DailyRewardUI popup created
- [ ] Popup shows on app launch
- [ ] Can claim rewards
- [ ] Streak system works
- [ ] Resets after missed day
- [ ] Analytics tracking events
- [ ] UI looks good
- [ ] Sounds play
- [ ] Tested all 7 days
- [ ] Tested streak break
- [ ] Ready to ship! 🚀

---

**Daily Rewards = One of the HIGHEST ROI features you can add!**

10 hours of work → +50% retention → +100% revenue

DO IT! 💰
