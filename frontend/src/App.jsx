import { useEffect, useState } from "react";
import { Layout, Card, Statistic, Tag, Typography, Space, Row, Col } from "antd";
import boilerImage from "./assets/boiler.jpg";

const { Header, Content, Footer } = Layout;
const { Title, Text } = Typography;

function App() {
  const [twin, setTwin] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTwin = async () => {
      try {
        const res = await fetch("http://localhost:5000/twin");
        if (!res.ok) throw new Error("HTTP " + res.status);
        const data = await res.json();
        setTwin(data);
        setError(null);
      } catch (err) {
        console.error("Fetch error:", err);
        setError("API'ye bağlanılamadı: " + err.message);
      }
    };

    fetchTwin();
    const id = setInterval(fetchTwin, 1000);
    return () => clearInterval(id);
  }, []);

  const isAlarm =
    twin &&
    (twin.overheated ||
      twin.currentTemperature > twin.targetTemperature);

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Header style={{ background: "#001529" }}>
        <Title
          level={2}
          style={{ color: "white", margin: 0, textAlign: "center" }}
        >
          Digital Twin Dashboard
        </Title>
      </Header>

      <Content style={{ padding: "24px", background: "#f0f2f5" }}>
        <div style={{ maxWidth: 800, margin: "0 auto" }}>
          <Card
            title="Live PLC Data"
            style={{
              borderRadius: 16,
              boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
              border: isAlarm ? "2px solid #ff4d4f" : "1px solid #f0f0f0",
            }}
          >
            {!twin && !error && <Text>Loading...</Text>}
            {error && <Text type="danger">{error}</Text>}

            {twin && (
              <Row gutter={24} align="middle">
                {/* left side: values */}
                <Col span={16}>
                  <Space direction="vertical" size="large" style={{ width: "100%" }}>
                    <Statistic
                      title="Current Temperature (°C)"
                      value={twin.currentTemperature}
                      precision={1}
                    />
                    <Statistic
                      title="Target Temperature (°C)"
                      value={twin.targetTemperature}
                      precision={1}
                    />
                    <div>
                      <Text strong>Overheated: </Text>
                      {twin.overheated ? (
                        <Tag color="red">YES</Tag>
                      ) : (
                        <Tag color="green">NO</Tag>
                      )}
                    </div>

                    {isAlarm && (
                      <div
                        style={{
                          padding: "12px",
                          borderRadius: 8,
                          background: "#fff1f0",
                          border: "1px solid #ffa39e",
                        }}
                      >
                        <Tag color="red" style={{ marginBottom: 8 }}>
                          ALARM
                        </Tag>
                        <Text type="danger">
                          Process overheating! Check boiler conditions.
                        </Text>
                      </div>
                    )}

                    <Text type="secondary">
                      Last update:{" "}
                      {new Date(twin.timestamp).toLocaleTimeString()}
                    </Text>
                  </Space>
                </Col>

                {/* right side image */}
                <Col span={8} style={{ textAlign: "center" }}>
                  <img
                    src={boilerImage}
                    alt="Boiler"
                    style={{
                      width: "100%",
                      maxWidth: "240px",
                      borderRadius: 12,
                      filter: isAlarm ? "drop-shadow(0 0 10px red)" : "none",
                    }}
                  />
                </Col>
              </Row>
            )}
          </Card>
        </div>
      </Content>

      <Footer style={{ textAlign: "center" }}>
        Edge Digital Twin Demo
      </Footer>
    </Layout>
  );
}

export default App;
