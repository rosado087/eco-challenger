export interface ChallengeListModel {
  success: boolean,
  message: string,
  dailyChallenges: {challenge: object, wasConcluded: boolean}[],
  weeklyChallenges: {challenge:object, progress: Number, wasConcluded: boolean}[]
}
