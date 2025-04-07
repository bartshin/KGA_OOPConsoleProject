namespace ConsoleProject;

static class Assets {
  public const string Intro = "책임감 있는 시민이자 가족을 사랑하는 가장, 테드의 행복한 교외 생활에 약간의 문제가 발생했습니다. 핵폭발로 세상이 멸망한 것이죠. 남은 시간은 60초, 이 상황에서 테드는 미친 듯이 극렬하게 집안을 휘젓고 다니며 가족들과 유용한 물자를 챙깁니다. 모든 일이 쉽지 않을 것입니다. 시간은 촉박한데 아끼던 가구들이 방해를 하고, 매번 집의 모습이 바뀝니다. 과연 무엇을 가져가고, 누구를 놔두고 가야할까요? 제때 방공호에 도달해 살아남더라도 그것은 단지 시작에 불과합니다. 집어온 것들과 데려온 사람들은 모두 당신의 생존에 중요한 역할을 합니다. 각 생존 이야기는 매일 예상치 못한 사건과 함께 달라집니다. 이 이야기는 과연 해피엔딩이 될까요? 당신에게 달려있습니다. 식량과 물을 배급하고, 물자를 최대한 잘 활용하고, 어려운 결정을 하고, 황무지로 모험을 떠나기도 해야 합니다. 행운을 빕니다. 1950년대를 무대로 펼쳐지는 다크 코메디, 우리 동네에 핵무기가 떨어졌다!! 평범한 일상에 닥친 악몽을 체험해 보세요! 60초 안에 생존에 필요한 물자를 모으고, 집을 돌아다니며 신속하게 가족들을 구하세요. 최악의 상황에 대비하십시오. 물건들을 막 집는다고 좋은 게 아닙니다. 생존을 위해 계획을 세우고 긴급 방송의 유용한 정보를 따르십시오! 방공호에 가지고 온 물건들로 어떻게든 살아남아야 합니다. 며칠이나 살아남을 수 있을까요? 모두 살아서 나갈 수 있을까요? 멸망한 세상이 당신을 몰아세울 때, 어떻게 해야 할지 결정을 내려야 합니다. 위험을 감수하고 밖으로 나갈까요? 식량이 부족할 때 누가 저녁을 먹지 않아야 할까요? 돌연변이 바퀴벌레가 꼬이면 어떻게 할까요?";

  public const string TitleImage = """
    ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓▓▒▒▒▒▒▒▒▒░▒▒ ▒▒▒▒▒
    ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▒▒▒▒▒▒ ▒▒▒▒▒▒▒
    ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░▓░▒ ▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒
    ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░▓▓▓▓▓▓▓▒░░▓▓▓▓▓▒▓▓▓▒▒▒▒▒▒▒▒▒
    ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░        ░▓▓▓▓████████████▒        █▓▓▓▓▓▓▓▓▓▓▓ ░▓▓▓░▓▓▓ ░▓▓▓▓▒▓▓▒▒▒▒▒▒▒
    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒ ▒▓▓▓▓▓▓▓▓▓▓▓▓▒ ▓█████████  ▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓░▒▓░▓ ▓░▓▓▓ ▓▓▓▓▓▓▒▒▒▒▒
    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ██████ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░ ▓▓▓▓▓▓▓▓▓▓ ░▓▓ ▓░▓▓▒▓▓▓▓▓▓▓▓▒▒▒
    ▓▓░▓▓░▓▓░▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ████ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ █▓▓▓▓░░▓░▓░▓▓░▓▓▓▓▓▒▓▓▓▓▓▓▓▓▒▒
    ▓▓▓▓▓░▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▒ ██▓ ▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓  █▓▓▒▓▓░░░░▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▒
    ▓▓▒▓▒░▒▒▒▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓  ░▒▓▓▓▓▓▓▓▓▓ ██ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ███▓▓█▓▓ ░▓▓░░▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓
    ▓▓▓▓▓░▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓  ░ ▓▓▓▓▓▓▓▓▓ ▒█ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ██░▓▓▓█▓░░▓▓▒░▓▓▓░░▒▒░▓▓▓▓▓▓▓
    ▓▓▓▓▓░░▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓  ░     ▒▒▒▒▒▓██ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ██████▓▓░░▓▓▓▓▓▓▓░░▒▒▒░▓▒▓▓▓░
    ▓▓▓▓▓░▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓ ▓████ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ████▒█▓▓ ░▓▓▓ ▓▒▒░░▒▒▒▒░▒▒  ░
    ▓▓▓▓▓░▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ███ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ███▒▒█▓▓░░▓▓▓   ░▒░  ░░  ░   
    ▓▓▓▓▓░▒▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░ ██ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ▒▒█▒▒▒██   ▓ ░░ ░▓▒░░░  ░ ░░░
    ░▓▓▓▓░░▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓▓   ▓▓▓▓▓▓▓▓▓▓ ██ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓▓ ▒▒░▒▒▒▒ ▒░░░ ░░ ░░ ░░░░ ░░░ ░
    ░░▒░░░░░▒░░▒▒░ ▓▓▓▓▓▓▓▓▓   ▒▓▓▓▓▓▓▓▓▓ ▓█ ▓▓▓▓▓▓▓▓▓░ ░▒▓▓▓▓▓▓▓▓░  ▒▒░░ ░░░▒░░ ░░ ░░ ░░░░ ░ ░  
    ░░░░░░░ ░░░▒░▒ ▓▓▓▓▓▓▓▓▓  ░░▓▓▓▓▓▓▓▓▓ ░█ ▓▓▓▓▓▓▓▓▓░  ▒▓▓▓ ░░ ░░░░░░░░▒░░▒    ░▒ ░▒░          
    ░   ░░░░░░░░░░ ▓▓▓▓▓▓▓▓▓  ░░▓▓▓▓▓▓▓▓▓ ░▒ ▓▓▓▓▓▓░▓▓░ ░░░▒ ░░░▒░░ ░░ ░░░░░░░▒ ░░░  ▒▒░         
         ░░░░░░░░░ ▓▓▓▓▓▓▓▓▓   ░▓▓▓▓▓▓▒ ▒    ░░░▓ ░░░▒ ▒░░░░░░░░░▒░ ░░ ░░▒  ░░░░░░ ░░░▒░         
         ░░░░░░░░░ ▓▓▓▓▓▓▓▓▓   ░▓▓▓▓ ░░░░░░░░░░░ ░░░░░░░░░ ░░░░░░░░ ░░ ░░ ░  ░░▒░░░  ░▒▒▒░       
    ░░░░░░░░░░░░░░ ▓▓▓▓▓▓▓▓▓   ▓▓▓▓░░░░░░░░░░   ░░░ ▒ ░░░  ░░░░░░░░ ░░░░▒ ░░░░░ ░░░ ░▒▒▒░░       
    ░░░░░░░░░░░░░░ ▓▓▓▓▓▓▓▓▓▓ ▓▓▓▓▒░░░░   ░░░░░ ░░░ ▓ ░░ ░░░░▒░▒░░░ ░░░░   ░░            ▒░      
    ░░░░░░░░░░░░░░░ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▒░░░░░░░░░  ░░ ░░░░░░░░▒ ░░░░░░ ▒     ▓▓ ▓░ ▓ ▓   ░░░░░░░░░░ 
    ░░░░░░░░░░░░░░ ░  ▓▓▓▓▓▓▓▓▓▓▓▓▓░ ▓ ░░░░░░ ░░▒░░░▒  ░░░        ▓ ▓▓ ▓ ░▓ ▓  ▓▓░  ░░░░▒░░ ░░░░░
    ░░░░░░░░░░░░░░░░░░   ░▓▓▓▓▓▓▓▓░ ░░░░░░ ░░░░░  ▒      ▓▓▓▓▓▓▓░▓▓▓▓░▓ ▓ ▓▓▓▒    ░░░░░░  ░  ░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░       ░░ ▒░░        ▒▓▓▓ ▓▓  ░▓  ▓ ▓ ▓▓▓▓ ▓          ░▒▒▒░▒ ░░░░  ░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░      ▓▓   ▓▓     ▓ ▓  ▓▒  ▓▓▓▓▒               ░░   ░░ ░░░░░░▒░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ▓▓ ▓▓  ▓▓▓▓   ▓▓▓▓  ▓           ▒▒▒  ░░░░░░░░░░░ ░░░░░░░░░░░
    ░░░░░░░░ ░░░░░░░░░░░░░░░░░░░░   ▒▓▓▓░   ▓▓ ▓▓▓ ▓        ▒   ▒▒▒▒   ░░░░░░░░░░░░░░░░░░     ░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ▓▓ ░▓▓ ▓▓▓░        ░░ ░░░ ▒▒▒          ░░░░░░░░░░░░░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ▓              ░░░░░ ░░░░░░  ░ ░▒▒░▒▒▒▒▒░▒░░░░░░░░░░░░░ ░░░░░
     ░░░░░░░░░░░░░░░░░░░░░░░░░░░        ░░░░░ ░░░░░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒▒▒▒░░░ ░░░  ░░░░░ ░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░    ░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░▒▒▒░▒▒▒▒░░░░░░░░░░░░░░░░░░░   ░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▒░ ░▒░  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒░░░░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▒░▒▒▒▒░ ░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░ ▒▒▒▒░░░░       ░░░░ ░░░░░ 
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░░ ░░░░░░░░░░░░░░░░▒▒░░░░░░░░░░░ ░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  ░░░░░░░░░░ ░░░▒░░░░░░░░░░░░░░░░░
    ░░░░░░░ ░░░ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░░ ░░░░░░░░░░░░░░░░░░
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░░ ░░░░░░░░░░░░░░░ 
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░ ░░░░░░░░░░     
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░░░       ░░   
    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  ░  ░░░░░░░ ░░░░  ░░░░░░░░░░ ░░     ░░

    """;
}

