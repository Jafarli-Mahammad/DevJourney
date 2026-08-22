import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 10,
    duration: '10s',
};

export default function () {
    const res = http.get('http://localhost:5074/api/partner/Competitions');
    check(res, {
        'status is 401 (unauthorized)': (r) => r.status === 401,
    });
    sleep(1);
}
