class Solution {
    public int[] solution(int[] arr) {
        // 1 개만 남았으면 규격대로 -1 리턴
        if (arr.length == 1) {
            return new int[] { -1 };
        }

        // 배열에서 최솟값 찾기
        int min = arr[0];
        for (int n : arr) { // foreach 사용
            if (n < min) {
                min = n;
            }
        }

        // 최솟값을 제외한 원소로 새 배열 만들기
        int[] answer = new int[arr.length - 1];
        int idx = 0;
        for (int n : arr) {
            if (n != min) {
                answer[idx++] = n;
            }
        }
        return answer;
    }
}

// 프로그래머스 java 문제. 입력값 {4, 3, 2, 1} => {4, 3, 2}, {10} => {-1}