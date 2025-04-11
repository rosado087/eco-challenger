import { ChallengeModel } from "./challenge-model"

export interface GetChallengeResponse {
  success: boolean
  message?: string
  challenge: ChallengeModel
}