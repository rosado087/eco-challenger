import { ChallengeModel } from "./challenges-model"

export interface ChallengeResponse {
    success: boolean
    dailyChallengesToDo: ChallengeModel[]
    dailyChallengesDone: ChallengeModel[]
    weeklyChallengesToDo: ChallengeModel[]
    weeklyChallengesDone: ChallengeModel[]
    message?: string
  }