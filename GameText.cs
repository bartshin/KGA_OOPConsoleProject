using System.Text;

namespace ConsoleProject;

static class GameText {

  static public string AddDayText(int totalDay) {
    StringBuilder builder = new (string.Format($"{totalDay}일차\n"));
    switch (totalDay) {
      case 1:
        builder.Append("""
            우리는 폭발 직전에 방공호로 들어왔다
            간발의 차였지! 모두가 함께 살아남을 수는 없었지만 
            어떻게든 여기서 살아남을거다
            """); 
          break;
      case 5:
        builder.Append(string.Format($"이미 {totalDay}일이나 되었다 언제쯤 나갈 수 있을까"));
        break;
      case 3:
        builder.Append(string.Format($"벌써 {totalDay}일이 지났다 슬슬 불안하다"));
        break;
      default:
        break;
    }
    return (builder.ToString());
  }

  static public string AddCosumableItemText(string name, double amount) {
    if (amount < 1) {
      return string.Format($"{name}이 다 떨어졌다!");
    }
    if (amount < 2) {
      return string.Format($"{name}이 얼마 없다!");
    }
    if (amount < 3) {
      return string.Format($"{name}이 오래가지 못할 것 같다. 아껴서 사용해야겠다.");
    }
    if (amount < 4) {
      return string.Format($"{name}이 그렇게 넉넉하지는 않다 엄격하게 배급해야겠다.");
    }
    return string.Format($"당분간 {name} 걱정은 없다.");
  }

  static public string GetCharacterComment(Character.Status? status) {
    if (status == null)
      return string.Format($"건강하고 활력이 넘친다!");
    switch (status) {
      case Character.Status.Hungry:
        return string.Format($"배가 고프다.");
      case Character.Status.Starvation:
        return string.Format($"굶주렸다!");
      case Character.Status.Thirsty:
        return string.Format($"목이 마르다.");
      case Character.Status.Dehydration:
        return string.Format($"탈수 증세가 있다!");
      case Character.Status.Sick:
        return string.Format($"몸이 아프다.");
      case Character.Status.Crazy:
        return string.Format($"정신 착란 증세가 있다!");
      case Character.Status.Fatigued:
        return string.Format($"심한 부상을 입었다!");
      case Character.Status.Hurt:
        return string.Format($"통증을 느끼고 있다.");
      case Character.Status.Tired:
        return string.Format($"피곤하다.");
      default: 
        throw new NotImplementedException();
    }
  }
}
